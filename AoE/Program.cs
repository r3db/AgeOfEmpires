using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace AoE
{

    internal class Program
    {
        private const int Repeat = 40;
        private const bool IsVerbose = false;

        private static readonly IDictionary<int, string> _palettes = new Dictionary<int, string>
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

        private static void Main()
        {
            var path = @"C:\Users\r3db\Desktop\a_alfred_attackA_x2.smx";
            //var path = @"C:\Users\r3db\Desktop\u_sie_siege_ram_walkA_x2.smx";
            //var path = @"C:\Users\r3db\Desktop\b_medi_barracks_age3_x2.smx";

            using (var br = new BinaryReader(File.OpenRead(path)))
            {
                var smxHeader = ReadSMXHeader(br);

                for (int i = 0; i < smxHeader.FrameCount; i++)
                {   
                    var smxFrameHeader = ReadSMXFrameHeader(br);
                    var smxLayers      = ReadSMXLayers(smxFrameHeader, br);

                    //if (smxFrameHeader.HasGraphics())
                    //{
                    //    smxLayers[0].Graphics.Image.Save($@"C:\Users\r3db\Desktop\a\test_g_{i}.png");
                    //}

                    //if (smxFrameHeader.HasShadow())
                    //{
                    //    smxLayers[1].Graphics.Image.Save($@"C:\Users\r3db\Desktop\a\test_s_{i}.png");
                    //}

                    //if (smxFrameHeader.HasOutline())
                    //{
                    //    smxLayers[2].Graphics.Image.Save($@"C:\Users\r3db\Desktop\a\test_o_{i}.png");
                    //}

                    var width   = smxLayers.Max(x => x.Header.Width);
                    var height  = smxLayers.Max(x => x.Header.Height);
                    var anchorX = smxLayers.Max(x => x.Header.AnchorX);
                    var anchorY = smxLayers.Max(x => x.Header.AnchorY);

                    var bmp = new Bitmap(width, height);
                    var canvas = Graphics.FromImage(bmp);
                    //canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    canvas.FillRectangle(new SolidBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue)), 0, 0, width, height);

                    if (smxFrameHeader.HasGraphics())
                    {
                        canvas.DrawImage(smxLayers[0].Graphics.Image, new Point(-(smxLayers[0].Header.AnchorX - anchorX), -(smxLayers[0].Header.AnchorY - anchorY)));
                    }

                    if (smxFrameHeader.HasShadow())
                    {
                        canvas.DrawImage(smxLayers[1].Graphics.Image, new Point(-(smxLayers[1].Header.AnchorX - anchorX), -(smxLayers[1].Header.AnchorY - anchorY)));
                    }

                    canvas.Save();

                    bmp.Save($@"C:\Users\r3db\Desktop\a\test_c_{i}.png");
                }

                Console.WriteLine("{0}", new string('-', Repeat));
            }
        }

        // Helpers
        private static SMXHeader ReadSMXHeader(BinaryReader reader)
        {
            var magic       = Encoding.ASCII.GetString(reader.ReadBytes(4));
            var version     = reader.ReadInt16();
            var frameCount  = reader.ReadInt16();
            var fileSizeSMX = reader.ReadInt32();
            var fileSizeSMP = reader.ReadInt32();
            var comment     = Encoding.ASCII.GetString(reader.ReadBytes(16));

            var header = new SMXHeader
            {
                Descriptor  = magic,
                Version     = version,
                FrameCount  = frameCount,
                FileSizeSMX = fileSizeSMX,
                FileSizeSMP = fileSizeSMP,
                Comment     = comment,
            };

            if (IsVerbose)
            {
                Console.WriteLine("Header");
                Console.WriteLine("{0}", new string('-', Repeat));
                Console.WriteLine("Magic         : '{0}'", header.Descriptor);
                Console.WriteLine("Version       : {0}", header.Version);
                Console.WriteLine("Frame Count   : {0}", header.FrameCount);
                Console.WriteLine("File Size SMX : {0} kb", header.FileSizeSMX / 1024);
                Console.WriteLine("File Size SMP : {0} kb", header.FileSizeSMP / 1024);
                Console.WriteLine("Comment       : '{0}'", header.Comment);
            }

            return header;
        }

        private static SMXFrameHeader ReadSMXFrameHeader(BinaryReader reader)
        {
            var frameType        = (SMXFrameType)reader.ReadByte();
            var paletteNumber    = reader.ReadByte();
            var uncompressedSize = reader.ReadInt32();

            var compression = ((int)frameType & 0b_0000_1000) == 0
                ? SMXFrameCompression.FourPlusOne
                : SMXFrameCompression.EightToFive;

            var smxFrameHeader = new SMXFrameHeader
            {
                Kind             = (SMXFrameType)frameType,
                Compression      = compression,
                PaletteNumber    = paletteNumber,
                UncompressedSize = uncompressedSize,
            };

            if (IsVerbose)
            {
                Console.WriteLine("\tFrame Header");
                Console.WriteLine("\t{0}", new string('-', Repeat));
                Console.WriteLine("\tFrameType         : [{0}], Compression: {1}", smxFrameHeader.Kind, smxFrameHeader.Compression);
                Console.WriteLine("\tPalette Number    : {0}, '{1}'", smxFrameHeader.PaletteNumber, _palettes[paletteNumber]);
                Console.WriteLine("\tUncompressed Size : {0} bytes", smxFrameHeader.UncompressedSize);
                Console.WriteLine("\t{0}", new string('-', Repeat));
            }

            return smxFrameHeader;
        }

        private static IList<SMXLayer> ReadSMXLayers(SMXFrameHeader smxFrameHeader, BinaryReader reader)
        {
            var result = new List<SMXLayer>();

            if (smxFrameHeader.HasGraphics())
            {
                var smxLayerHeader       = ReadSMXLayerHeader(reader);
                var smxLayerRowEdgeArray = ReadSMXLayerRowEdgeArray(smxLayerHeader, reader);
                var graphics             = ReadGraphics(smxLayerHeader, smxFrameHeader.PaletteNumber, smxFrameHeader.Compression, smxLayerRowEdgeArray, reader);

                result.Add(new SMXLayer
                {
                    Header   = smxLayerHeader,
                    Edges    = smxLayerRowEdgeArray,
                    Graphics = graphics,
                });
            }

            if (smxFrameHeader.HasShadow())
            {
                var smxLayerHeader       = ReadSMXLayerHeader(reader);
                var smxLayerRowEdgeArray = ReadSMXLayerRowEdgeArray(smxLayerHeader, reader);
                var graphics             = ReadShadow(smxLayerHeader, smxLayerRowEdgeArray, reader);

                result.Add(new SMXLayer
                {
                    Header   = smxLayerHeader,
                    Edges    = smxLayerRowEdgeArray,
                    Graphics = graphics,
                });
            }

            if (smxFrameHeader.HasOutline())
            {
                var smxLayerHeader       = ReadSMXLayerHeader(reader);
                var smxLayerRowEdgeArray = ReadSMXLayerRowEdgeArray(smxLayerHeader, reader);
                var graphics             = ReadOutline(smxLayerHeader, smxLayerRowEdgeArray, reader);

                result.Add(new SMXLayer
                {
                    Header   = smxLayerHeader,
                    Edges    = smxLayerRowEdgeArray,
                    Graphics = graphics,
                });
            }

            return result;
        }

        private static SMXLayerHeader ReadSMXLayerHeader(BinaryReader reader)
        {
            var width       = reader.ReadUInt16();
            var height      = reader.ReadUInt16();
            var anchorX     = reader.ReadInt16();
            var anchorY     = reader.ReadInt16();
            var layerLength = reader.ReadUInt32();
            var _           = reader.ReadUInt32();

            var smxLayerHeader = new SMXLayerHeader
            {
                Width       = width,
                Height      = height,
                AnchorX     = anchorX,
                AnchorY     = anchorY,
                LayerLength = layerLength
            };

            if (IsVerbose)
            {
                Console.WriteLine("\tLayer Header");
                Console.WriteLine("\t{0}", new string('-', Repeat));
                Console.WriteLine("\tSize         : {0} x {1} ({2}, {3})", smxLayerHeader.Width, smxLayerHeader.Height, smxLayerHeader.AnchorX, smxLayerHeader.AnchorY);
                Console.WriteLine("\tLayer Length : {0}", smxLayerHeader.LayerLength);
                Console.WriteLine("\tUnknown      : {0}", _);
                Console.WriteLine("\t{0}", new string('-', Repeat));
            }

            return smxLayerHeader;
        }

        private static IList<SMXLayerRowEdge> ReadSMXLayerRowEdgeArray(SMXLayerHeader header, BinaryReader reader)
        {
            var result = new List<SMXLayerRowEdge>();

            for (int i = 0; i < header.Height; i++)
            {
                result.Add(ReadSMXLayerRowEdge(reader, header.Width));
            }

            return result;
        }

        private static SMXLayerRowEdge ReadSMXLayerRowEdge(BinaryReader reader, int width)
        {
            var leftSpacing  = reader.ReadUInt16();
            var rightSpacing = reader.ReadUInt16();
            var isEmpty      = leftSpacing == 0xffff || rightSpacing == 0xffff;

            if (isEmpty == false && (leftSpacing + rightSpacing > width))
            {
                throw new InvalidDataException();
            }

            var smxLayerRowEdge = new SMXLayerRowEdge
            {
                LeftSpacing  = leftSpacing,
                RightSpacing = rightSpacing,
            };

            if (IsVerbose)
            {
                Console.WriteLine("\t\tLayer Row Edge");
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
                Console.WriteLine("\t\tSpacing  : ({0,2}, {1,2}), IsEmpy: {2}", leftSpacing, rightSpacing, isEmpty);
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
            }

            return smxLayerRowEdge;
        }

        private static SMXGraphics ReadGraphics(SMXLayerHeader header, int paletteIndex, SMXFrameCompression compression, IList<SMXLayerRowEdge> rowEdges, BinaryReader reader)
        {
            var commandLength = reader.ReadUInt32();
            var pixelsLength  = reader.ReadUInt32();
            var commands      = reader.ReadBytes((int)commandLength);
            var pixels        = reader.ReadBytes((int)pixelsLength);
            var decoder       = new SMXPixelDecoder(pixels, paletteIndex, compression);

            if (IsVerbose)
            {
                Console.WriteLine("\t\tGraphics Layer");
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
                Console.WriteLine("\t\tCommand Length : {0}", commandLength);
                Console.WriteLine("\t\tPixels Length  : {0}", pixelsLength);

                Console.WriteLine("\t\t\tCommand Array");
                Console.WriteLine("\t\t\t{0}", new string('-', Repeat));
            }

            var x = rowEdges[0].LeftSpacing;
            var y = 0;

            using (var brc = new BinaryReader(new MemoryStream(commands)))
            using (var brp = new BinaryReader(new MemoryStream(pixels)))
            {
                var result = new Bitmap(header.Width, header.Height);

                for (int i = 0; i < commands.Length; i++)
                {
                    if (rowEdges[y].IsEmpty())
                    {
                        // Todo: Draw Transparent Line!
                        x = rowEdges[++y].LeftSpacing;
                        continue;
                    }

                    if (rowEdges[y].LeftSpacing != 0 || rowEdges[y].RightSpacing != 0)
                    {
                        // Todo: Draw Transparent 'Spacing'!
                    }

                    var command = brc.ReadByte();
                    var opCode = (SMXCommandOpCode)(command & 0b00000011);
                    var payload = (command >> 2) + 1;

                    switch (opCode)
                    {
                        case SMXCommandOpCode.Draw:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Draw ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, decoder.Next());
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.Player:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Player ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                decoder.Next();
                                result.SetPixel(x, y, Color.Blue);
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.Skip:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Skip ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, Color.Transparent);
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.End:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : End", command);
                            }

                            if (i + 1 < commands.Length)
                            {
                                x = rowEdges[++y].LeftSpacing;
                            }

                            break;
                        }
                        default:
                        {
                            throw new InvalidDataException();
                        }
                    }
                }

                if (IsVerbose)
                {
                    Console.WriteLine("\t\t\t{0}", new string('-', Repeat));
                }

                return new SMXGraphics
                {
                    CommandData = commands,
                    PixelData   = pixels,
                    Image       = result,
                };
            }
        }

        private static SMXGraphics ReadShadow(SMXLayerHeader header, IList<SMXLayerRowEdge> rowEdges, BinaryReader reader)
        {
            var unifiedArrayLength = reader.ReadUInt32();
            var unifiedArray       = reader.ReadBytes((int)unifiedArrayLength);

            if (IsVerbose)
            {
                Console.WriteLine("\t\tShadow Layer");
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
                Console.WriteLine("\t\tUnified Array Length : {0}", unifiedArrayLength);
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
            }

            var x = rowEdges[0].LeftSpacing;
            var y = 0;

            using (var br = new BinaryReader(new MemoryStream(unifiedArray)))
            {
                var result = new Bitmap(header.Width, header.Height);

                for (int i = 0; i < unifiedArray.Length; i++)
                {
                    if (rowEdges[y].IsEmpty())
                    {
                        // Todo: Draw Transparent Line!
                        x = rowEdges[++y].LeftSpacing;
                        continue;
                    }

                    if (x == 0 && (rowEdges[y].LeftSpacing != 0 || rowEdges[y].RightSpacing != 0))
                    {
                        // Todo: Draw Transparent 'Spacing'!
                    }

                    var command = br.ReadByte();
                    var opCode = (SMXCommandOpCode)(command & 0b00000011);
                    var payload = (command >> 2) + 1;

                    switch (opCode)
                    {
                        case SMXCommandOpCode.Draw:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Draw ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            var ap = payload;
                            i += ap;
                            var w = br.ReadBytes(ap);

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, Color.FromArgb(w[k], 0, 0, 0));
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.Skip:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Skip ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, Color.Transparent);
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.End:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : End", command);
                            }

                            if (i + 1 < unifiedArray.Length)
                            {
                                x = rowEdges[++y].LeftSpacing;
                            }

                            break;
                        }
                        default:
                        {
                            throw new InvalidDataException();
                        }
                    }
                }

                if (IsVerbose)
                {
                    Console.WriteLine("\t\t\t{0}", new string('-', Repeat));
                }

                return new SMXGraphics
                {
                    CommandData = null,
                    PixelData   = unifiedArray,
                    Image       = result,
                };
            }
        }

        private static SMXGraphics ReadOutline(SMXLayerHeader header, IList<SMXLayerRowEdge> rowEdges, BinaryReader reader)
        {
            var unifiedArrayLength = reader.ReadUInt32();
            var unifiedArray       = reader.ReadBytes((int)unifiedArrayLength);

            if (IsVerbose)
            {
                Console.WriteLine("\t\tOutline Layer");
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
                Console.WriteLine("\t\tUnified Array Length : {0}", unifiedArrayLength);
                Console.WriteLine("\t\t{0}", new string('-', Repeat));
            }

            var x = rowEdges[0].LeftSpacing;
            var y = 0;

            using (var br = new BinaryReader(new MemoryStream(unifiedArray)))
            {
                var result = new Bitmap(header.Width, header.Height);

                for (int i = 0; i < unifiedArray.Length; i++)
                {
                    if (rowEdges[y].IsEmpty())
                    {
                        // Todo: Draw Transparent Line!
                        x = rowEdges[++y].LeftSpacing;
                        continue;
                    }

                    if (x == 0 && (rowEdges[y].LeftSpacing != 0 || rowEdges[y].RightSpacing != 0))
                    {
                        // Todo: Draw Transparent 'Spacing'!
                    }

                    var command = br.ReadByte();
                    var opCode = (SMXCommandOpCode)(command & 0b00000011);
                    var payload = (command >> 2) + 1;

                    switch (opCode)
                    {
                        case SMXCommandOpCode.Draw:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Draw ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, Color.Blue);
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.Skip:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : Skip ({1,3} -> {2,3})", command, x, x + payload);
                            }

                            for (int k = 0; k < payload; k++)
                            {
                                result.SetPixel(x, y, Color.Transparent);
                                x += 1;
                            }

                            break;
                        }
                        case SMXCommandOpCode.End:
                        {
                            if (IsVerbose)
                            {
                                Console.WriteLine("\t\t\t[{0,3}] : End", command);
                            }

                            if (i + 1 < unifiedArray.Length)
                            {
                                x = rowEdges[++y].LeftSpacing;
                            }

                            break;
                        }
                        default:
                        {
                            throw new InvalidDataException();
                        }
                    }
                }

                if (IsVerbose)
                {
                    Console.WriteLine("\t\t\t{0}", new string('-', Repeat));
                }

                return new SMXGraphics
                {
                    CommandData = null,
                    PixelData   = unifiedArray,
                    Image       = result,
                };
            }
        }
    }
}