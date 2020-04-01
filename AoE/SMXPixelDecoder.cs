using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoE
{
    internal sealed class SMXPixelDecoder
    {
        private static readonly IDictionary<int, IList<Color>> _colors = new Dictionary<int, IList<Color>>();

        private static readonly IDictionary<int, string> _w = new Dictionary<int, string>
        {
            { 0, "original.pal"             },
            { 1, "clf_pal.pal"              },
            { 2, "pal_2.pal"                },
            { 3, "pal_3.pal"                },
            { 4, "pal_4.pal"                },
            { 5, "pal_5.pal"                },
            { 6, "pal_6.pal"                },
            { 16, "b_dark.pal"              },
            { 17, "b_orie.pal"              },
            { 18, "b_seas.pal"              },
            { 19, "b_ceas.pal"              },
            { 20, "b_east.pal"              },
            { 21, "b_west.pal"              },
            { 22, "b_asia.pal"              },
            { 23, "b_meso.pal"              },
            { 24, "b_slav.pal"              },
            { 25, "b_afri.pal"              },
            { 26, "b_indi.pal"              },
            { 27, "b_medi.pal"              },
            { 28, "b_scen.pal"              },
            { 29, "b_scen.pal"              },
            { 30, "n_trees.pal"             },
            { 31, "n_trees.pal"             },
            { 32, "n_alpha_ground.palx"     },
            { 33, "n_alpha_underwater.palx" },
            { 40, "n_cliffs.pal"            },
            { 41, "effects_2.pal"           },
            { 42, "b_scen.pal"              },
            { 54, "effects.pal"             },
            { 55, "playercolor_blue.pal"    },
            { 56, "playercolor_red.pal"     },
            { 57, "playercolor_green.pal"   },
            { 58, "playercolor_yellow.pal"  },
            { 59, "playercolor_orange.pal"  },
            { 60, "playercolor_teal.pal"    },
            { 61, "playercolor_purple.pal"  },
            { 62, "playercolor_grey.pal"    },
            { 63, "modulation_colors.palx"  },
        };

        private readonly Queue<Color> _pixels = new Queue<Color>();
        private readonly BinaryReader _reader;
        private readonly int _paletteIndex;
        private readonly SMXFrameCompression _compression;

        static SMXPixelDecoder()
        {
            foreach (var palette in _w)
            {
                var path   = @"C:\Users\r3db\Desktop\palettes\" + palette.Value;
                var lines  = File.ReadAllLines(path).Skip(3).ToList();
                var colors = new List<Color>();

                _colors.Add(palette.Key, colors);

                foreach (var line in lines.Where(x => x.Trim().StartsWith("$") == false && x.Trim().StartsWith("#") == false && x.Trim() == string.Empty == false))
                {
                    var components = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    colors.Add(Color.FromArgb(/*int.Parse(components[3])*/byte.MaxValue, int.Parse(components[0]), int.Parse(components[1]), int.Parse(components[2])));
                }
            }
        }

        internal SMXPixelDecoder(byte[] data, int paletteIndex, SMXFrameCompression compression)
        {
            _reader = new BinaryReader(new MemoryStream(data));
            _paletteIndex = paletteIndex;
            _compression = compression;
        }

        public int Index { get { return (int)_reader.BaseStream.Position; } }

        internal Color Next()
        {
            if (_pixels.Count == 0)
            {
                if (_compression == SMXFrameCompression.FourPlusOne)
                {
                    FourPlusOne();
                }
                else if (_compression == SMXFrameCompression.EightToFive)
                {
                    EightToFive();
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            return _pixels.Dequeue();
        }

        // Helpers
        private void FourPlusOne()
        {
            var data = _reader.ReadBytes(5);

            var ps0 = (data[4] & 0b_0000_0011) >> 0;
            var ps1 = (data[4] & 0b_0000_1100) >> 2;
            var ps2 = (data[4] & 0b_0011_0000) >> 4;
            var ps3 = (data[4] & 0b_1100_0000) >> 6;

            var ci0 = data[0] + 256 * ps0;
            var ci1 = data[1] + 256 * ps1;
            var ci2 = data[2] + 256 * ps2;
            var ci3 = data[3] + 256 * ps3;

            var c0 = _colors[_paletteIndex][ci0];
            var c1 = _colors[_paletteIndex][ci1];
            var c2 = _colors[_paletteIndex][ci2];
            var c3 = _colors[_paletteIndex][ci3];

            _pixels.Enqueue(c0);
            _pixels.Enqueue(c1);
            _pixels.Enqueue(c2);
            _pixels.Enqueue(c3);
        }

        private void EightToFive()
        {
            var data = _reader.ReadBytes(5);

            var pi0  = (data[0] & 0b_1111_1111) >> 0;
            var ps0  = (data[1] & 0b_0000_0011) >> 0;
            var dm10 = (data[2] & 0b_1111_0000) >> 4;
            var dm20 = (data[3] & 0b_0011_1111) >> 0;

            var pi1  = (data[2] & 0b_0000_0011) >> 0 | (data[1] & 0b_1111_1100) >> 0;
            var ps1  = (data[2] & 0b_0000_1100) >> 2;
            var dm11 = (data[4] & 0b_0000_0011) >> 0 | (data[3] & 0b_1100_0000) >> 2;
            var dm21 = (data[4] & 0b_1111_1100) >> 2;

            var ci0 = pi0 + 256 * ps0;
            var ci1 = pi1 + 256 * ps1;

            var c0 = _colors[_paletteIndex][ci0];
            var c1 = _colors[_paletteIndex][ci1];

            _pixels.Enqueue(c0);
            _pixels.Enqueue(c1);
        }
    }
}