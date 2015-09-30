using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace simple_console_gui
{
    public class ConsoleRenderer
    {
        char[] buffer;
        char[] oldBuffer;   //The buffer rendered previously, used to only draw what is needed.
        int[] foreColours;  //The foreground colours of the buffer
        int[] oldForeColours;   //The colours previously rendered
        int[] backColours;  //The background colours of the buffer
        int[] oldBackColours;   //The background colours previously rendered
        int bufferWidth;
        int bufferHeight;
        //private StreamWriter stdout;    //Used to write to the console using a stream to get the fastest write speed possible.

        public ConsoleRenderer(int bufferWidth, int bufferHeight)
        {
            this.bufferWidth = bufferWidth;
            this.bufferHeight = bufferHeight;
            foreColours = new int[bufferWidth * bufferHeight];
            backColours = new int[bufferWidth * bufferHeight];
            oldForeColours = new int[bufferWidth * bufferHeight];
            oldBackColours = new int[bufferWidth * bufferHeight];
            buffer = new char[bufferWidth * bufferHeight];
            oldBuffer = new char[bufferWidth * bufferHeight];
            //stdout = new StreamWriter(Console.OpenStandardOutput(), System.Text.Encoding.Default);
            //stdout.AutoFlush = false;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        private void ClearBuffer(ConsoleColor screenColour)
        {
            Array.Copy(buffer, oldBuffer, buffer.Length);
            Array.Copy(foreColours, oldForeColours, foreColours.Length);
            Array.Copy(backColours, oldBackColours, backColours.Length);
            Array.Clear(buffer, 0, buffer.Length);
            Array.Clear(foreColours, 0, foreColours.Length);

            for (int i = 0; i < backColours.Length; i++) {
                backColours[i] = (int)screenColour;
            }
        }

        /// <summary>
        /// Outputs the contents of the buffer to the console.
        /// </summary>
        private void DrawBuffer()
        {
            /*
            //Concatenate the buffer into one continuous string
            string output = "";
            for (int i = 0; i < buffer.Length; i++) {
                output += buffer[i];
                //stdout.Write(buffer[i]);
            }
            stdout.Write(output);
            stdout.Flush();
             * */

            for (int x = 0; x < bufferWidth; x++) {
                for (int y = 0; y < bufferHeight; y++) {
                    if (buffer[y * bufferWidth + x] != oldBuffer[y * bufferWidth + x]
                            || foreColours[y * bufferWidth + x] != oldForeColours[y * bufferWidth + x]
                            || backColours[y * bufferWidth + x] != oldBackColours[y * bufferWidth + x]) {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = (ConsoleColor)foreColours[y * bufferWidth + x];
                        Console.BackgroundColor = (ConsoleColor)backColours[y * bufferWidth + x];
                        Console.Write(buffer[y * bufferWidth + x]);
                    }
                }
            }

        }

        /// <summary>
        /// Call this to output the buffer to the screen and then clear it.
        /// </summary>
        public void Paint(ConsoleColor screenColour)
        {
            DrawBuffer();
            ClearBuffer(screenColour);
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Writes a character to the buffer at the specified location.
        /// </summary>
        /// <param name="x">The x co-ordinate to write to</param>
        /// <param name="y">The y co-ordinate to write to</param>
        /// <param name="c">The character to write</param>
        private int WriteChar(int x, int y, char c, ConsoleColor foreColour, ConsoleColor backColour)
        {
            try {
                buffer[y * bufferWidth + x] = c;
                foreColours[y * bufferWidth + x] = (int)foreColour;
                backColours[y * bufferWidth + x] = (int)backColour;
                return 0;
            } catch {
                return 1;
            }
        }

        public void WriteString(int x, int y, string text, ConsoleColor foreColour, ConsoleColor backColour)
        {
            char[] characters = text.ToCharArray();

            for (int i = 0; i < characters.Length; i++) {
                if (WriteChar(x + i, y, characters[i], foreColour, backColour) == 1)
                    return;
            }
        }

        /// <summary>
        /// Draws a box at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="border"></param>
        public void DrawBox(int x, int y, int width, int height, BorderStyles border, ConsoleColor foreColour, ConsoleColor backColour)
        {
            switch (border) {
                case BorderStyles.oneLine:
                    //Place corners
                    WriteChar(x, y, '┌', foreColour, backColour);
                    WriteChar(x + (width - 1), y, '┐', foreColour, backColour);
                    WriteChar(x + (width - 1), y + (height - 1), '┘', foreColour, backColour);
                    WriteChar(x, y + (height - 1), '└', foreColour, backColour);
                    //Place edges
                    for (int tempY = y + 1; tempY < y + (height - 1); tempY++) {
                        WriteChar(x, tempY, '│', foreColour, backColour);
                        WriteChar(x + (width - 1), tempY, '│', foreColour, backColour);
                    }
                    for (int tempX = x + 1; tempX < x + (width - 1); tempX++) {
                        WriteChar(tempX, y, '─', foreColour, backColour);
                        WriteChar(tempX, y + (height - 1), '─', foreColour, backColour);
                    }
                    break;

                case BorderStyles.twoLine:
                    //Place corners
                    WriteChar(x, y, '╔', foreColour, backColour);
                    WriteChar(x + (width - 1), y, '╗', foreColour, backColour);
                    WriteChar(x + (width - 1), y + (height - 1), '╝', foreColour, backColour);
                    WriteChar(x, y + (height - 1), '╚', foreColour, backColour);
                    //Place edges
                    for (int tempY = y + 1; tempY < y + (height - 1); tempY++) {
                        WriteChar(x, tempY, '║', foreColour, backColour);
                        WriteChar(x + (width - 1), tempY, '║', foreColour, backColour);
                    }
                    for (int tempX = x + 1; tempX < x + (width - 1); tempX++) {
                        WriteChar(tempX, y, '═', foreColour, backColour);
                        WriteChar(tempX, y + (height - 1), '═', foreColour, backColour);
                    }
                    break;
            }
        }
    }
}
