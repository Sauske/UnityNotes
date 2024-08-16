namespace Gif
{
    using System;
    using System.IO;

    public class LZW_Decompress
    {
        private byte[] block = new byte[0x100];
        private int blockSize;
        private int MaxStackSize = 0x1000;
        private byte[] pixelStack;
        private short[] prefix;
        private byte[] suffix;

        public ERROR Decompress(int iw, int ih, ref byte[] pixels, BinaryReader reader)
        {
            int num10;
            int num12;
            int num16;
            int num17;
            int num18;
            int num19;
            int num = -1;
            int num2 = iw * ih;
            if ((pixels == null) || (pixels.Length < num2))
            {
                pixels = new byte[num2];
            }
            if (this.prefix == null)
            {
                this.prefix = new short[this.MaxStackSize];
            }
            if (this.suffix == null)
            {
                this.suffix = new byte[this.MaxStackSize];
            }
            if (this.pixelStack == null)
            {
                this.pixelStack = new byte[this.MaxStackSize + 1];
            }
            int num15 = reader.ReadByte();
            int num4 = ((int) 1) << num15;
            int num7 = num4 + 1;
            int index = num4 + 2;
            int num9 = num;
            int num6 = num15 + 1;
            int num5 = (((int) 1) << num6) - 1;
            int num11 = 0;
            while (num11 < num4)
            {
                this.prefix[num11] = 0;
                this.suffix[num11] = (byte) num11;
                num11++;
            }
            int num14 = num10 = num12 = num16 = num17 = num19 = num18 = 0;
            int num13 = 0;
            while (num13 < num2)
            {
                if (num17 == 0)
                {
                    if (num10 < num6)
                    {
                        if (num12 == 0)
                        {
                            num12 = this.ReadBlock(reader);
                            if (num12 <= 0)
                            {
                                break;
                            }
                            num18 = 0;
                        }
                        num14 += (this.block[num18] & 0xff) << num10;
                        num10 += 8;
                        num18++;
                        num12--;
                        continue;
                    }
                    num11 = num14 & num5;
                    num14 = num14 >> num6;
                    num10 -= num6;
                    if ((num11 > index) || (num11 == num7))
                    {
                        break;
                    }
                    if (num11 == num4)
                    {
                        num6 = num15 + 1;
                        num5 = (((int) 1) << num6) - 1;
                        index = num4 + 2;
                        num9 = num;
                        continue;
                    }
                    if (num9 == num)
                    {
                        this.pixelStack[num17++] = this.suffix[num11];
                        num9 = num11;
                        num16 = num11;
                        continue;
                    }
                    int num8 = num11;
                    if (num11 == index)
                    {
                        this.pixelStack[num17++] = (byte) num16;
                        num11 = num9;
                    }
                    while (num11 > num4)
                    {
                        this.pixelStack[num17++] = this.suffix[num11];
                        num11 = this.prefix[num11];
                    }
                    num16 = this.suffix[num11] & 0xff;
                    if (index >= this.MaxStackSize)
                    {
                        break;
                    }
                    this.pixelStack[num17++] = (byte) num16;
                    this.prefix[index] = (short) num9;
                    this.suffix[index] = (byte) num16;
                    index++;
                    if (((index & num5) == 0) && (index < this.MaxStackSize))
                    {
                        num6++;
                        num5 += index;
                    }
                    num9 = num8;
                }
                num17--;
                pixels[num19++] = this.pixelStack[num17];
                num13++;
            }
            for (num13 = num19; num13 < num2; num13++)
            {
                pixels[num13] = 0;
            }
            return ERROR.OK;
        }

        protected int ReadBlock(BinaryReader reader)
        {
            this.blockSize = reader.ReadByte();
            if (this.blockSize > 0)
            {
                this.block = reader.ReadBytes(this.blockSize);
            }
            return this.blockSize;
        }
    }
}

