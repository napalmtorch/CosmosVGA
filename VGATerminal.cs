using System;
using Cosmos.System.Graphics;

namespace Cosmos.HAL
{
    public static class VGATerminal
    {
        // cursor
        public static int CursorX { get; private set; }
        public static int CursorY { get; private set; }

        // colors
        public static ConsoleColor BackColor = ConsoleColor.Black;
        public static ConsoleColor TextColor = ConsoleColor.White;

        // clear the screen
        public static void Clear()
        {
            VGADriverII.Clear((byte)BackColor);
            SetCursorPos(0, 0);
        }

        // draw character to position on screen
        public static unsafe void PutCharacter(int x, int y, char c, ConsoleColor fg, ConsoleColor bg)
        {
            uint index = (uint)(x + (y * VGADriverII.Width)) * 2;
            VGADriverII.Buffer[index] = (byte)c;
            VGADriverII.Buffer[index + 1] = ToAttribute((VGAColor)fg, (VGAColor)bg);
        }

        // print character to next position
        public static void WriteChar(char c, ConsoleColor fg, ConsoleColor bg)
        {
            if (c == '\n') { NewLine(); }
            else
            {
                PutCharacter(CursorX, CursorY, c, fg, bg);
                CursorX++;
                if (CursorX >= VGADriverII.Width) { NewLine(); }
                UpdateCursor();
            }
        }
        public static void WriteChar(char c, ConsoleColor fg) { WriteChar(c, fg, BackColor); }
        public static void WriteChar(char c) { WriteChar(c, TextColor, BackColor); }

        // print string to next position
        public static void Write(string text, ConsoleColor fg, ConsoleColor bg)
        {
            for (int i = 0; i < text.Length; i++)
            {
                WriteChar(text[i], fg, bg);
            }
        }
        public static void Write(string text, ConsoleColor fg) { Write(text, fg, BackColor); }
        public static void Write(string text) { Write(text, TextColor, BackColor); }

        // print line to next positoin
        public static void WriteLine(string text, ConsoleColor fg, ConsoleColor bg) { Write(text + "\n", fg, bg); }
        public static void WriteLine(string text, ConsoleColor fg) { WriteLine(text, fg, BackColor); }
        public static void WriteLine(string text) { WriteLine(text, TextColor, BackColor); }

        // backspace input
        private static void Backspace()
        {
            if (CursorX > 0)
            {
                SetCursorX(CursorX - 1);
                PutCharacter(CursorX, CursorY, ' ', TextColor, BackColor);
            }
            else if (CursorY > 0)
            {
                SetCursorPos(VGADriverII.Width - 1, CursorY - 1);
                PutCharacter(CursorX, CursorY, ' ', TextColor, BackColor);
            }
        }

        // read line of input
        public static string ReadLine()
        {
            string input = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // character
                if (key.KeyChar >= 32 && key.KeyChar <= 126) { WriteChar(key.KeyChar); input += key.KeyChar; }
                // backspace
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1, 1);
                        Backspace();
                    }
                }
                // enter
                else if (key.Key == ConsoleKey.Enter) { WriteChar('\n'); break; }
            }
            return input;
        }

        // generate new line
        public static void NewLine()
        {
            CursorY++;
            if (CursorY >= VGADriverII.Height)
            {
                Scroll();
                SetCursorPos(0, VGADriverII.Height - 1);
            }
            else { SetCursorPos(0, CursorY); }
        }

        // scroll by one line
        private static unsafe void Scroll()
        {
            //VGADriverII.MemoryCopy(VGADriverII.Buffer + (VGADriverII.Width * 2), VGADriverII.Buffer, VGADriverII.Width * (VGADriverII.Height - 1) * 2);
            
            for (int i = 0; i < VGADriverII.Width; i++)
                PutCharacter(i, VGADriverII.Height - 1, ' ', TextColor, BackColor);
        }

        // set cursor position
        public static unsafe void SetCursorPos(int x, int y)
        {
            VGADriverII.SetCursorPos((ushort)x, (ushort)y);
            CursorX = x; CursorY = y;
            VGADriverII.Buffer[((x + (y * VGADriverII.Width)) * 2) + 1] = ToAttribute((VGAColor)TextColor, (VGAColor)BackColor);
        }
        public static void SetCursorX(int x) { SetCursorPos(x, CursorY); }
        public static void SetCursorY(int y) { SetCursorPos(CursorX, y); }
        public static void UpdateCursor() { SetCursorPos(CursorX, CursorY); }
        public static void DisableCursor() { VGADriverII.DisableCursor(); }

        // convert colors to attribute
        public static byte ToAttribute(VGAColor fg, VGAColor bg) { return (byte)((byte)fg | (byte)bg << 4); }

    }
}
