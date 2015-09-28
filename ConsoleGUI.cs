using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BorderStyles
{
    none,
    oneLine,
    twoLine,
}

namespace broadside_client_windows
{
    /// <summary>
    /// This class provides methods to create and manage GUI-like elements in a console environment.
    /// </summary>
    public class ConsoleGUI
    {

        ConsoleRenderer consoleRenderer;
        List<Element> screen;
        int numberOfElements;
        int activeElement;

        ConsoleColor screenColour { get; set; }

        /// <summary>
        /// Instantiates a new console GUI, creating a consoleRenderer and an array to hold the GUI elements.
        /// </summary>
        public ConsoleGUI(ConsoleColor screenColour = ConsoleColor.Black)
        {
            consoleRenderer = new ConsoleRenderer(Console.WindowWidth, Console.WindowHeight);
            screen = new List<Element>(); //This list contains every element to be drawn to the screen. Elements are rendered in the order they're added to the list.
            numberOfElements = -1;
            activeElement = -1;
            this.screenColour = screenColour;
        }

        /// <summary>
        /// Draws all the elements to the screen.
        /// </summary>
        public void PaintScreen() 
        {
            foreach (Element currentElement in screen) {
                currentElement.DrawElement(consoleRenderer);
            }
            consoleRenderer.Paint(screenColour);
        }

        public string Interact()
        {
            return screen[activeElement].Interact();
        }

        public void Update()
        {
            foreach (Element currentElement in screen) {
                currentElement.Update();
            }
        }

        /// <summary>
        /// Adds an element to the screen
        /// </summary>
        public void AddElement(Element newElement, bool makeActive = false)
        {
            screen.Add(newElement);
            numberOfElements++;
            if (makeActive) {
                activeElement = numberOfElements++;
            }
        }

        /// <summary>
        /// This class provides the basic features of a GUI element that can then be inherited from.
        /// </summary>
        public abstract class Element
        {
            public int xPosition;  //The x position of the element
            public int yPosition;  //The y position of the element
            public int width;  //The width of the element in characters
            public int height; //The height of the element in characters
            public ConsoleColor foreColour; //The foreground colour of the element
            public ConsoleColor backColour; //The background colour of the element
            public BorderStyles border;    //The style of border of the element. Borders are always internal.

            public abstract void DrawElement(ConsoleRenderer consoleRenderer);  //Called to draw the element to the buffer.
            public abstract void DeleteElement();   //Called to remove the element when it is no longer needed.
            public abstract void Update();  //Called to update the element.
            public abstract string Interact();  //Called to interact with the element.

            internal Element()
            {
                //Used to stop the abstract class being instantiated
            }
        }

        /// <summary>
        /// This class fulfils the same basic purpose as the standard console; it stores lines in a buffer that can then be rendered.
        /// </summary>
        public class LogBox : Element
        {
            int bufferSize; //The size of the text buffer
            string[] buffer;    //The contents of the log box.
            
            public LogBox(int x, int y, int width, int height, BorderStyles border = BorderStyles.none, ConsoleColor foreColour = ConsoleColor.White, ConsoleColor backColour = ConsoleColor.Black)
            {
                this.xPosition = x;
                this.yPosition = y;
                this.width = width;
                this.height = height;
                this.border = border;

                this.foreColour = foreColour;
                this.backColour = backColour;

                bufferSize = height;
                buffer = new string[bufferSize];
                //fill the buffer with blank strings
                for (int i = 0; i < bufferSize; i++)
                    buffer[i] = "";
            }

            public void WriteLine(string lineToWrite)
            {
                string[] newBuffer = new string[bufferSize];    //Create a new buffer into which we will copy our existing buffer
                Array.Copy(buffer, 0, newBuffer, 1, bufferSize - 1);    //Copy over the contents of the current buffer, cutting off the last entry and leaving the first entry clear.
                newBuffer[0] = lineToWrite; //Write the new entry to the first element
                buffer = newBuffer; //Copy the temporary buffer into the buffer variable
            }

            public override void DrawElement(ConsoleRenderer consoleRenderer) 
            {
                if (border == BorderStyles.none) {
                    //no border, just draw the log.
                    WriteBuffer(0, consoleRenderer);
                }
                else {
                    //Draw a border before writing the text.
                    consoleRenderer.DrawBox(xPosition, yPosition, width, height, border, foreColour, backColour);
                    WriteBuffer(1, consoleRenderer);
                }
            }

            private void WriteBuffer(int borderOffset, ConsoleRenderer consoleRenderer)
            {
                int tempWidth = width - (borderOffset * 2);
                int tempHeight = height - (borderOffset * 2);
                int tempX = xPosition + borderOffset;
                int tempY = yPosition + borderOffset;

                int linesRemaining = tempHeight;
                int charsPerLine = tempWidth;
                for (int currentString = 0; currentString < bufferSize; currentString++) {
                    if (linesRemaining > 0) {
                        //For each line, calculate how many lines it will take up in the logbox.
                        int numberOfLines = (int)Math.Ceiling((double)buffer[currentString].Length / (double)charsPerLine);

                        if (numberOfLines < 2) {
                            //We only need to write one line; simple!
                            consoleRenderer.WriteString(tempX, tempY + (linesRemaining - 1), buffer[currentString], foreColour, backColour);
                            linesRemaining--;
                        }
                        else {
                            //This is where it gets awkward. We need to account for text wrapping.
                            //To do this we split up the string into several substrings that will fit in the Log Box.
                            string[] substringArray = new string[numberOfLines];
                            for (int c = 0; c < numberOfLines - 1; c++) {
                                substringArray[c] = buffer[currentString].Substring(c * charsPerLine, charsPerLine);
                            }
                            substringArray[numberOfLines - 1] = buffer[currentString].Substring((numberOfLines - 1) * charsPerLine);    //The last substring must go to the end of the string.

                            //Now, add each substring to the buffer and update the linesRemaining variable.
                            for (int c = numberOfLines - 1; c >= 0; c--) {
                                if (linesRemaining > 0) {
                                    consoleRenderer.WriteString(tempX, tempY + (linesRemaining - 1), substringArray[c], foreColour, backColour);
                                    linesRemaining--;
                                }
                            }
                        }
                    }
                }
            }

            public override void DeleteElement()
            {
                throw new NotImplementedException();
            }

            public override string Interact()
            {
                //Console.ReadLine();
                return "";
            }

            public override void Update()
            {
            }
        }
    }
}
