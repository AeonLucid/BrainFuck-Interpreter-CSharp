using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BrainFuk
{
    internal class Program
    {
        private static byte[] _program;

        private static ulong _programPointer;

        private static byte[] _memory;

        private static ulong _pointer;

        private static Stack<ulong> _loopPointers;
        
        private static Dictionary<ulong, ulong> _loopCache;

        private static Stopwatch stopwatch = Stopwatch.StartNew();

        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.ASCII;

            _program = File.ReadAllBytes("Files/hanoi.b");
            _programPointer = 0;
            _memory = new byte[1024];
            _pointer = 0;
            _loopPointers = new Stack<ulong>();
            _loopCache = new Dictionary<ulong, ulong>();

            // Run
            while (_programPointer < (ulong)_program.Length)
            {
                switch (_program[_programPointer])
                {
                    case 0x3E: // >
                        ++_pointer;
                        break;

                    case 0x3C: // <
                        --_pointer;
                        break;

                    case 0x2B: // +
                        ++_memory[_pointer];
                        break;

                    case 0x2D: // -
                        --_memory[_pointer];
                        break;

                    case 0x2E: // .
                        Console.Write(Convert.ToChar(_memory[_pointer]));
                        break;

                    case 0x2C: // ,
                        _memory[_pointer] = (byte)Console.Read();
                        break;

                    case 0x5B: // [
                        if (_memory[_pointer] != 0x00)
                        {
                            _loopPointers.Push(_programPointer);
                        }
                        else
                        {
                            if (_loopCache.ContainsKey(_programPointer))
                            {
                                _programPointer = _loopCache[_programPointer];
                            }
                            else
                            {
                                _programPointer++;

                                // Skip the loop.
                                var currentPointer = _programPointer;
                                var depth = 1;

                                for (var p = _programPointer; p < (ulong)_program.Length; p++)
                                {
                                    switch (_program[p])
                                    {
                                        case 0x5B:
                                            depth++;
                                            break;
                                        case 0x5D:
                                            depth--;
                                            break;
                                    }

                                    if (depth == 0)
                                    {
                                        _loopCache[currentPointer] = p;
                                        _programPointer = p;
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case 0x5D: // ]
                        var oldPointer = _programPointer;

                        if (_loopPointers.TryPop(out _programPointer))
                        {
                            _loopCache[_programPointer] = oldPointer;
                            _programPointer--;
                        }
                        break;
                }

                _programPointer++;
            }

            Console.WriteLine($"Done in {stopwatch.Elapsed}.");
        }
    }
}