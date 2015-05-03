﻿#region Usings

using System;

#endregion

#pragma warning disable 1591

namespace Imp.Image
{
    // nj_result_t: Result codes for njDecode().
    public enum nj_result_t
    {
        NJ_OK = 0, // no error, decoding successful
        NJ_NO_JPEG, // not a JPEG file
        NJ_UNSUPPORTED, // unsupported format
        NJ_OUT_OF_MEM, // out of memory
        NJ_INTERNAL_ERR, // internal error
        NJ_SYNTAX_ERROR, // syntax error
        __NJ_FINISHED, // used internally, will never be reported
    };

    public struct nj_vlc_code_t
    {
        #region Fields

        public byte bits;
        public byte code;

        #endregion
    };

    public class nj_component_t
    {
        #region Fields

        public int cid;
        public int ssx, ssy;
        public int width, height;
        public int stride;
        public int qtsel;
        public int actabsel, dctabsel;
        public int dcpred;
        public byte[] pixels;

        #endregion
    }

    public class nj_context_t
    {
        #region Fields

        public byte[] posb; // C#: Because we don't have fancy pointers.
        public nj_result_t error;
        public int pos;
        public int size;
        public int length;
        public int width, height;
        public int mbwidth, mbheight;
        public int mbsizex, mbsizey;
        public int ncomp;
        public nj_component_t[] comp;
        public int qtused, qtavail;
        public byte[][] qtab;
        public nj_vlc_code_t[][] vlctab;
        public int buf, bufbits;
        public int[] block;
        public int rstinterval;
        public byte[] rgb;

        #endregion

        public nj_context_t()
        {
            posb = null;
            comp = new nj_component_t[3];
            block = new int[64];
            qtab = new byte[4][];
            vlctab = new nj_vlc_code_t[4][];
            for (byte i = 0; i < 4; i++)
            {
                qtab[i] = new byte[64];
                vlctab[i] = new nj_vlc_code_t[65536];
                if (i < comp.Length)
                {
                    comp[i] = new nj_component_t();
                }
            }
        }
    }

    public class nj_exception : Exception {}

    public class NanoJpeg
    {
        #region Static Fields and Constants

        public static readonly byte[] njZZ = new byte[]
        {
            0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18,
            11, 4, 5, 12, 19, 26, 33, 40, 48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35,
            42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52, 45,
            38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62, 63
        };

        public static readonly int W1 = 2841;
        public static readonly int W2 = 2676;
        public static readonly int W3 = 2408;
        public static readonly int W5 = 1609;
        public static readonly int W6 = 1108;
        public static readonly int W7 = 565;
        public static readonly int CF4A = (-9);
        public static readonly int CF4B = (111);
        public static readonly int CF4C = (29);
        public static readonly int CF4D = (-3);
        public static readonly int CF3A = (28);
        public static readonly int CF3B = (109);
        public static readonly int CF3C = (-9);
        public static readonly int CF3X = (104);
        public static readonly int CF3Y = (27);
        public static readonly int CF3Z = (-3);
        public static readonly int CF2A = (139);
        public static readonly int CF2B = (-11);

        #endregion

        #region Fields

        public nj_context_t nj = new nj_context_t();

        #endregion

        public static byte njClip(int x)
        {
            return (byte) ((x < 0) ? 0 : ((x > 0xFF) ? 0xFF : (byte) x));
        }

        public void njRowIDCT(int[] blk, int coef)
        {
            int x0, x1, x2, x3, x4, x5, x6, x7, x8;
            if (((x1 = blk[coef + 4] << 11)
                 | (x2 = blk[coef + 6])
                 | (x3 = blk[coef + 2])
                 | (x4 = blk[coef + 1])
                 | (x5 = blk[coef + 7])
                 | (x6 = blk[coef + 5])
                 | (x7 = blk[coef + 3])) == 0)
            {
                blk[coef] = blk[coef + 1] = blk[coef + 2] = blk[coef + 3] = blk[coef + 4] = blk[coef + 5] = blk[coef + 6] = blk[coef + 7] = blk[coef] << 3;
                return;
            }
            x0 = (blk[coef] << 11) + 128;
            x8 = W7 * (x4 + x5);
            x4 = x8 + (W1 - W7) * x4;
            x5 = x8 - (W1 + W7) * x5;
            x8 = W3 * (x6 + x7);
            x6 = x8 - (W3 - W5) * x6;
            x7 = x8 - (W3 + W5) * x7;
            x8 = x0 + x1;
            x0 -= x1;
            x1 = W6 * (x3 + x2);
            x2 = x1 - (W2 + W6) * x2;
            x3 = x1 + (W2 - W6) * x3;
            x1 = x4 + x6;
            x4 -= x6;
            x6 = x5 + x7;
            x5 -= x7;
            x7 = x8 + x3;
            x8 -= x3;
            x3 = x0 + x2;
            x0 -= x2;
            x2 = (181 * (x4 + x5) + 128) >> 8;
            x4 = (181 * (x4 - x5) + 128) >> 8;
            blk[coef] = (x7 + x1) >> 8;
            blk[coef + 1] = (x3 + x2) >> 8;
            blk[coef + 2] = (x0 + x4) >> 8;
            blk[coef + 3] = (x8 + x6) >> 8;
            blk[coef + 4] = (x8 - x6) >> 8;
            blk[coef + 5] = (x0 - x4) >> 8;
            blk[coef + 6] = (x3 - x2) >> 8;
            blk[coef + 7] = (x7 - x1) >> 8;
        }

        public void njColIDCT(int[] blk, int coef, byte[] pixels, int outv, int stride)
        {
            int x0, x1, x2, x3, x4, x5, x6, x7, x8;
            if (((x1 = blk[coef + 8 * 4] << 8)
                 | (x2 = blk[coef + 8 * 6])
                 | (x3 = blk[coef + 8 * 2])
                 | (x4 = blk[coef + 8 * 1])
                 | (x5 = blk[coef + 8 * 7])
                 | (x6 = blk[coef + 8 * 5])
                 | (x7 = blk[coef + 8 * 3])) == 0)
            {
                x1 = njClip(((blk[coef] + 32) >> 6) + 128);
                for (x0 = 8; x0 != 0; --x0)
                {
                    pixels[outv] = (byte) x1;
                    outv += stride;
                }
                return;
            }
            x0 = (blk[coef] << 8) + 8192;
            x8 = W7 * (x4 + x5) + 4;
            x4 = (x8 + (W1 - W7) * x4) >> 3;
            x5 = (x8 - (W1 + W7) * x5) >> 3;
            x8 = W3 * (x6 + x7) + 4;
            x6 = (x8 - (W3 - W5) * x6) >> 3;
            x7 = (x8 - (W3 + W5) * x7) >> 3;
            x8 = x0 + x1;
            x0 -= x1;
            x1 = W6 * (x3 + x2) + 4;
            x2 = (x1 - (W2 + W6) * x2) >> 3;
            x3 = (x1 + (W2 - W6) * x3) >> 3;
            x1 = x4 + x6;
            x4 -= x6;
            x6 = x5 + x7;
            x5 -= x7;
            x7 = x8 + x3;
            x8 -= x3;
            x3 = x0 + x2;
            x0 -= x2;
            x2 = (181 * (x4 + x5) + 128) >> 8;
            x4 = (181 * (x4 - x5) + 128) >> 8;
            pixels[outv] = njClip(((x7 + x1) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x3 + x2) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x0 + x4) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x8 + x6) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x8 - x6) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x0 - x4) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x3 - x2) >> 14) + 128);
            outv += stride;
            pixels[outv] = njClip(((x7 - x1) >> 14) + 128);
        }

        public void njThrow(nj_result_t e)
        {
            nj.error = e;
            throw new nj_exception();
        }

        public int njShowBits(int bits)
        {
            byte newbyte;
            if (bits == 0) return 0;
            while (nj.bufbits < bits)
            {
                if (nj.size <= 0)
                {
                    nj.buf = (nj.buf << 8) | 0xFF;
                    nj.bufbits += 8;
                    continue;
                }
                newbyte = nj.posb[nj.pos++];
                nj.size--;
                nj.bufbits += 8;
                nj.buf = (nj.buf << 8) | newbyte;
                if (newbyte == 0xFF)
                {
                    if (nj.size != 0)
                    {
                        var marker = nj.posb[nj.pos++];
                        nj.size--;
                        switch (marker)
                        {
                            case 0x00:
                            case 0xFF:
                                break;
                            case 0xD9:
                                nj.size = 0;
                                break;
                            default:
                                if ((marker & 0xF8) != 0xD0)
                                    nj.error = nj_result_t.NJ_SYNTAX_ERROR;
                                else
                                {
                                    nj.buf = (nj.buf << 8) | marker;
                                    nj.bufbits += 8;
                                }
                                break;
                        }
                    }
                    else
                        nj.error = nj_result_t.NJ_SYNTAX_ERROR;
                }
            }
            return (nj.buf >> (nj.bufbits - bits)) & ((1 << bits) - 1);
        }

        public void njSkipBits(int bits)
        {
            if (nj.bufbits < bits)
                njShowBits(bits);
            nj.bufbits -= bits;
        }

        public int njGetBits(int bits)
        {
            var res = njShowBits(bits);
            njSkipBits(bits);
            return res;
        }

        public void njByteAlign()
        {
            nj.bufbits &= 0xF8;
        }

        public void njSkip(int count)
        {
            nj.pos += count;
            nj.size -= count;
            nj.length -= count;
            if (nj.size < 0) nj.error = nj_result_t.NJ_SYNTAX_ERROR;
        }

        public ushort njDecode16(int pos)
        {
            return (ushort) ((nj.posb[pos] << 8) | nj.posb[pos + 1]);
        }

        public void njDecodeLength()
        {
            if (nj.size < 2) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            nj.length = njDecode16(nj.pos);
            if (nj.length > nj.size) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            njSkip(2);
        }

        public void njSkipMarker()
        {
            njDecodeLength();
            njSkip(nj.length);
        }

        public void njDecodeSOF()
        {
            int i, ssxmax = 0, ssymax = 0;
            nj_component_t c;
            njDecodeLength();
            if (nj.length < 9) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            if (nj.posb[nj.pos] != 8) njThrow(nj_result_t.NJ_UNSUPPORTED);
            nj.height = njDecode16(nj.pos + 1);
            nj.width = njDecode16(nj.pos + 3);
            nj.ncomp = nj.posb[nj.pos + 5];
            njSkip(6);
            switch (nj.ncomp)
            {
                case 1:
                case 3:
                    break;
                default:
                    njThrow(nj_result_t.NJ_UNSUPPORTED);
                    break;
            }
            if (nj.length < (nj.ncomp * 3)) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            for (i = 0; i < nj.ncomp; ++i)
            {
                c = nj.comp[i];
                c.cid = nj.posb[nj.pos];
                if ((c.ssx = nj.posb[nj.pos + 1] >> 4) == 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                if ((c.ssx & (c.ssx - 1)) != 0) njThrow(nj_result_t.NJ_UNSUPPORTED); // non-power of two
                if ((c.ssy = nj.posb[nj.pos + 1] & 15) == 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                if ((c.ssy & (c.ssy - 1)) != 0) njThrow(nj_result_t.NJ_UNSUPPORTED); // non-power of two
                if (((c.qtsel = nj.posb[nj.pos + 2]) & 0xFC) != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                njSkip(3);
                nj.qtused |= 1 << c.qtsel;
                if (c.ssx > ssxmax) ssxmax = c.ssx;
                if (c.ssy > ssymax) ssymax = c.ssy;
            }
            if (nj.ncomp == 1)
            {
                c = nj.comp[0];
                c.ssx = c.ssy = ssxmax = ssymax = 1;
            }
            nj.mbsizex = ssxmax << 3;
            nj.mbsizey = ssymax << 3;
            nj.mbwidth = (nj.width + nj.mbsizex - 1) / nj.mbsizex;
            nj.mbheight = (nj.height + nj.mbsizey - 1) / nj.mbsizey;
            for (i = 0; i < nj.ncomp; ++i)
            {
                c = nj.comp[i];
                c.width = (nj.width * c.ssx + ssxmax - 1) / ssxmax;
                c.stride = (c.width + 7) & 0x7FFFFFF8;
                c.height = (nj.height * c.ssy + ssymax - 1) / ssymax;
                c.stride = nj.mbwidth * nj.mbsizex * c.ssx / ssxmax;
                if (((c.width < 3) && (c.ssx != ssxmax)) || ((c.height < 3) && (c.ssy != ssymax))) njThrow(nj_result_t.NJ_UNSUPPORTED);
                if ((c.pixels = new byte[c.stride * (nj.mbheight * nj.mbsizey * c.ssy / ssymax)]) == null) njThrow(nj_result_t.NJ_OUT_OF_MEM);
            }
            if (nj.ncomp == 3)
            {
                nj.rgb = new byte[nj.width * nj.height * nj.ncomp];
                if (nj.rgb == null) njThrow(nj_result_t.NJ_OUT_OF_MEM);
            }
            njSkip(nj.length);
        }

        public void njDecodeDHT()
        {
            int codelen, currcnt, remain, spread, i, j;
            nj_vlc_code_t[] vlc;
            int vlcc;
            var counts = new byte[16];
            njDecodeLength();
            while (nj.length >= 17)
            {
                i = nj.posb[nj.pos];
                if ((i & 0xEC) != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                if ((i & 0x02) != 0) njThrow(nj_result_t.NJ_UNSUPPORTED);
                i = (i | (i >> 3)) & 3; // combined DC/AC + tableid value
                for (codelen = 1; codelen <= 16; ++codelen)
                    counts[codelen - 1] = nj.posb[nj.pos + codelen];
                njSkip(17);
                vlc = nj.vlctab[i];
                vlcc = 0;
                remain = spread = 65536;
                for (codelen = 1; codelen <= 16; ++codelen)
                {
                    spread >>= 1;
                    currcnt = counts[codelen - 1];
                    if (currcnt == 0) continue;
                    if (nj.length < currcnt) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                    remain -= currcnt << (16 - codelen);
                    if (remain < 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                    for (i = 0; i < currcnt; ++i)
                    {
                        var code = nj.posb[nj.pos + i];
                        for (j = spread; j != 0; --j)
                        {
                            if (vlcc < 65536)
                            {
                                vlc[vlcc].bits = (byte) codelen;
                                vlc[vlcc].code = code;
                                vlcc++;
                            }
                        }
                    }
                    njSkip(currcnt);
                }
                while (remain-- != 0)
                {
                    if (vlcc < 65536)
                    {
                        vlc[vlcc].bits = 0;
                        vlcc++;
                    }
                }
            }
            if (nj.length != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
        }

        public void njDecodeDQT()
        {
            int i;
            byte[] t;
            njDecodeLength();
            while (nj.length >= 65)
            {
                i = nj.posb[nj.pos];
                if ((i & 0xFC) != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                nj.qtavail |= 1 << i;
                t = nj.qtab[i];
                for (i = 0; i < 64; ++i)
                    t[i] = nj.posb[nj.pos + i + 1];
                njSkip(65);
            }
            if (nj.length != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
        }

        public void njDecodeDRI()
        {
            njDecodeLength();
            if (nj.length < 2) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            nj.rstinterval = njDecode16(nj.pos);
            njSkip(nj.length);
        }

        public int njGetVLC(nj_vlc_code_t[] vlc, ref byte code)
        {
            var value = njShowBits(16);
            int bits = vlc[value].bits;
            if (bits == 0)
            {
                nj.error = nj_result_t.NJ_SYNTAX_ERROR;
                return 0;
            }
            njSkipBits(bits);
            value = vlc[value].code;
            code = (byte) value;
            bits = value & 15;
            if (bits == 0) return 0;
            value = njGetBits(bits);
            if (value < (1 << (bits - 1)))
                value += ((-1) << bits) + 1;
            return value;
        }

        public void njDecodeBlock(nj_component_t c, int outv)
        {
            byte discard = 0;
            byte code = 0;
            int value, coef = 0;
            nj.block = new int[64];
            c.dcpred += njGetVLC(nj.vlctab[c.dctabsel], ref discard);
            nj.block[0] = (c.dcpred) * nj.qtab[c.qtsel][0];
            do
            {
                value = njGetVLC(nj.vlctab[c.actabsel], ref code);
                if (code == 0) break; // EOB
                if ((code & 0x0F) == 0 && (code != 0xF0)) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                coef += (code >> 4) + 1;
                if (coef > 63) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                nj.block[(int) njZZ[coef]] = value * nj.qtab[c.qtsel][coef];
            } while (coef < 63);
            for (coef = 0; coef < 64; coef += 8)
                njRowIDCT(nj.block, coef);
            for (coef = 0; coef < 8; ++coef)
                njColIDCT(nj.block, coef, c.pixels, outv + coef, c.stride);
        }

        public void njDecodeScan()
        {
            int i, mbx, mby, sbx, sby;
            int rstcount = nj.rstinterval, nextrst = 0;
            nj_component_t c;
            njDecodeLength();
            if (nj.length < (4 + 2 * nj.ncomp)) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
            if (nj.posb[nj.pos] != nj.ncomp) njThrow(nj_result_t.NJ_UNSUPPORTED);
            njSkip(1);
            for (i = 0; i < nj.ncomp; ++i)
            {
                c = nj.comp[i];
                if (nj.posb[nj.pos] != c.cid) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                if ((nj.posb[nj.pos + 1] & 0xEE) != 0) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                c.dctabsel = nj.posb[nj.pos + 1] >> 4;
                c.actabsel = (nj.posb[nj.pos + 1] & 1) | 2;
                njSkip(2);
            }
            if (nj.posb[nj.pos] != 0 || (nj.posb[nj.pos + 1] != 63) || nj.posb[nj.pos + 2] != 0) njThrow(nj_result_t.NJ_UNSUPPORTED);
            njSkip(nj.length);
            for (mbx = mby = 0;;)
            {
                for (i = 0; i < nj.ncomp; ++i)
                {
                    c = nj.comp[i];
                    for (sby = 0; sby < c.ssy; ++sby)
                    {
                        for (sbx = 0; sbx < c.ssx; ++sbx)
                        {
                            njDecodeBlock(c, ((mby * c.ssy + sby) * c.stride + mbx * c.ssx + sbx) << 3);
                            if (nj.error != nj_result_t.NJ_OK) njThrow(nj.error);
                        }
                    }
                }
                if (++mbx >= nj.mbwidth)
                {
                    mbx = 0;
                    if (++mby >= nj.mbheight) break;
                }
                if (nj.rstinterval != 0 && (--rstcount) != 0)
                {
                    njByteAlign();
                    i = njGetBits(16);
                    if (((i & 0xFFF8) != 0xFFD0) || ((i & 7) != nextrst)) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                    nextrst = (nextrst + 1) & 7;
                    rstcount = nj.rstinterval;
                    for (i = 0; i < 3; ++i)
                        nj.comp[i].dcpred = 0;
                }
            }
            nj.error = nj_result_t.__NJ_FINISHED;
        }

        public static byte CF(int x)
        {
            return njClip(((x) + 64) >> 7);
        }

        public void njUpsampleH(nj_component_t c)
        {
            var xmax = c.width - 3;
            byte[] outv;
            int lin = 0, lout = 0;
            int x, y;
            outv = new byte[(c.width * c.height) << 1];
            if (outv == null) njThrow(nj_result_t.NJ_OUT_OF_MEM);
            for (y = c.height; y != 0; --y)
            {
                outv[lout] = CF(CF2A * c.pixels[lin] + CF2B * c.pixels[lin + 1]);
                outv[lout + 1] = CF(CF3X * c.pixels[lin] + CF3Y * c.pixels[lin + 1] + CF3Z * c.pixels[lin + 2]);
                outv[lout + 2] = CF(CF3A * c.pixels[lin] + CF3B * c.pixels[lin + 1] + CF3C * c.pixels[lin + 2]);
                for (x = 0; x < xmax; ++x)
                {
                    outv[lout + (x << 1) + 3] = CF(CF4A * c.pixels[lin + x] + CF4B * c.pixels[lin + x + 1] + CF4C * c.pixels[lin + x + 2] + CF4D * c.pixels[lin + x + 3]);
                    outv[lout + (x << 1) + 4] = CF(CF4D * c.pixels[lin + x] + CF4C * c.pixels[lin + x + 1] + CF4B * c.pixels[lin + x + 2] + CF4A * c.pixels[lin + x + 3]);
                }
                lin += c.stride;
                lout += c.width << 1;
                outv[lout + -3] = CF(CF3A * c.pixels[lin - 1] + CF3B * c.pixels[lin - 2] + CF3C * c.pixels[lin - 3]);
                outv[lout + -2] = CF(CF3X * c.pixels[lin - 1] + CF3Y * c.pixels[lin - 2] + CF3Z * c.pixels[lin - 3]);
                outv[lout + -1] = CF(CF2A * c.pixels[lin - 1] + CF2B * c.pixels[lin - 2]);
            }
            c.width <<= 1;
            c.stride = c.width;
            c.pixels = outv;
        }

        public void njUpsampleV(nj_component_t c)
        {
            int w = c.width, s1 = c.stride, s2 = s1 + s1;
            byte[] outv;
            int cin, cout;
            int x, y;
            outv = new byte[(c.width * c.height) << 1];
            if (outv == null) njThrow(nj_result_t.NJ_OUT_OF_MEM);
            for (x = 0; x < w; ++x)
            {
                cin = x;
                cout = x;
                outv[cout] = CF(CF2A * c.pixels[cin] + CF2B * c.pixels[cin + s1]);
                cout += w;
                outv[cout] = CF(CF3X * c.pixels[cin] + CF3Y * c.pixels[cin + s1] + CF3Z * c.pixels[cin + s2]);
                cout += w;
                outv[cout] = CF(CF3A * c.pixels[cin] + CF3B * c.pixels[cin + s1] + CF3C * c.pixels[cin + s2]);
                cout += w;
                cin += s1;
                for (y = c.height - 3; y != 0; --y)
                {
                    outv[cout] = CF(CF4A * c.pixels[cin + -s1] + CF4B * c.pixels[cin] + CF4C * c.pixels[cin + s1] + CF4D * c.pixels[cin + s2]);
                    cout += w;
                    outv[cout] = CF(CF4D * c.pixels[cin + -s1] + CF4C * c.pixels[cin] + CF4B * c.pixels[cin + s1] + CF4A * c.pixels[cin + s2]);
                    cout += w;
                    cin += s1;
                }
                cin += s1;
                outv[cout] = CF(CF3A * c.pixels[cin] + CF3B * c.pixels[cin - s1] + CF3C * c.pixels[cin - s2]);
                cout += w;
                outv[cout] = CF(CF3X * c.pixels[cin] + CF3Y * c.pixels[cin - s1] + CF3Z * c.pixels[cin - s2]);
                cout += w;
                outv[cout] = CF(CF2A * c.pixels[cin] + CF2B * c.pixels[cin - s1]);
            }
            c.height <<= 1;
            c.stride = c.width;
            c.pixels = outv;
        }

        public void njConvert()
        {
            int i;
            nj_component_t c;
            for (i = 0; i < nj.ncomp; ++i)
            {
                c = nj.comp[i];
                while ((c.width < nj.width) || (c.height < nj.height))
                {
                    if (c.width < nj.width) njUpsampleH(c);
                    if (nj.error != nj_result_t.NJ_OK) return;
                    if (c.height < nj.height) njUpsampleV(c);
                    if (nj.error != nj_result_t.NJ_OK) return;
                }
                if ((c.width < nj.width) || (c.height < nj.height)) njThrow(nj_result_t.NJ_INTERNAL_ERR);
            }
            if (nj.ncomp == 3)
            {
                // convert to RGB
                int x, yy;
                int prgb = 0, py = 0, pcb = 0, pcr = 0;
                for (yy = nj.height; yy != 0; --yy)
                {
                    for (x = 0; x < nj.width; ++x)
                    {
                        var y = nj.comp[0].pixels[py + x] << 8;
                        var cb = nj.comp[1].pixels[pcb + x] - 128;
                        var cr = nj.comp[2].pixels[pcr + x] - 128;
                        nj.rgb[prgb++] = njClip((y + 359 * cr + 128) >> 8);
                        nj.rgb[prgb++] = njClip((y - 88 * cb - 183 * cr + 128) >> 8);
                        nj.rgb[prgb++] = njClip((y + 454 * cb + 128) >> 8);
                    }
                    py += nj.comp[0].stride;
                    pcb += nj.comp[1].stride;
                    pcr += nj.comp[2].stride;
                }
            }
            else if (nj.comp[0].width != nj.comp[0].stride)
            {
                // grayscale . only remove stride
                var pin = nj.comp[0].stride;
                var pout = nj.comp[0].width;
                int y;
                for (y = nj.comp[0].height - 1; y != 0; --y)
                {
                    Buffer.BlockCopy(nj.comp[0].pixels,
                        pout,
                        nj.comp[0].pixels,
                        pin,
                        nj.comp[0].width);
                    pin += nj.comp[0].stride;
                    pout += nj.comp[0].width;
                }
                nj.comp[0].stride = nj.comp[0].width;
            }
        }

        public void njInit()
        {
            nj = new nj_context_t();
        }

        public nj_result_t njDecode(byte[] jpeg)
        {
            try
            {
                njInit();
                nj.posb = jpeg;
                nj.pos = 0;
                nj.size = jpeg.Length & 0x7FFFFFFF;
                if (nj.size < 2) njThrow(nj_result_t.NJ_NO_JPEG);
                if (((nj.posb[nj.pos] ^ 0xFF) | (nj.posb[nj.pos + 1] ^ 0xD8)) != 0) njThrow(nj_result_t.NJ_NO_JPEG);
                njSkip(2);
                while (nj.error == nj_result_t.NJ_OK)
                {
                    if ((nj.size < 2) || (nj.posb[nj.pos] != 0xFF)) njThrow(nj_result_t.NJ_SYNTAX_ERROR);
                    njSkip(2);
                    switch (nj.posb[nj.pos - 1])
                    {
                        case 0xC0:
                            njDecodeSOF();
                            break;
                        // case 0xC2: njDecodeSOF(); break;
                        case 0xC4:
                            njDecodeDHT();
                            break;
                        case 0xDB:
                            njDecodeDQT();
                            break;
                        case 0xDD:
                            njDecodeDRI();
                            break;
                        case 0xDA:
                            njDecodeScan();
                            break;
                        case 0xFE:
                            njSkipMarker();
                            break;
                        default:
                            if ((nj.posb[nj.pos - 1] & 0xF0) == 0xE0)
                                njSkipMarker();
                            else
                                njThrow(nj_result_t.NJ_UNSUPPORTED);
                            break;
                    }
                }
                if (nj.error != nj_result_t.__NJ_FINISHED) return nj.error;
                nj.error = nj_result_t.NJ_OK;
                njConvert();
                return nj.error;
            }
            catch (nj_exception)
            {
                return nj.error;
            }
        }

        public int njGetWidth()
        {
            return nj.width;
        }

        public int njGetHeight()
        {
            return nj.height;
        }

        public bool njIsColor()
        {
            return nj.ncomp != 1;
        }

        public byte[] njGetImage()
        {
            return nj.ncomp == 1 ? nj.comp[0].pixels : nj.rgb;
        }

        public int njGetImageSize()
        {
            return nj.width * nj.height * nj.ncomp;
        }
    }
}

#pragma warning restore 1591