﻿using Epi;
using ERHMS.EpiInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test
{
    public class TestProgram : Wrapper
    {
        internal static void Main(string[] args)
        {
            Settings.Default.RootDirectory = Environment.CurrentDirectory;
            try
            {
                MainBase(typeof(TestProgram), args);
            }
            finally
            {
                File.Delete(Configuration.DefaultConfigurationPath);
            }
        }

        public static Wrapper OutTest()
        {
            return Create(args => Main_OutTest(args));
        }
        private static void Main_OutTest(string[] args)
        {
            Console.Out.WriteLine("Standard output should be redirected to a null stream.");
            Out.WriteLine("Hello, world!");
        }

        public static Wrapper InAndOutTest()
        {
            return Create(args => Main_InAndOutTest(args));
        }
        private static void Main_InAndOutTest(string[] args)
        {
            if (Console.In.Peek() != -1)
            {
                throw new InvalidOperationException("Standard input should be redirected to a null stream.");
            }
            while (true)
            {
                string line = In.ReadLine();
                if (line == null)
                {
                    break;
                }
                Out.WriteLine(line);
            }
        }

        public static Wrapper ArgsTest(IEnumerable<int> values)
        {
            return Create(args => Main_ArgsTest(args), values.Select(value => value.ToString()).ToArray());
        }
        private static void Main_ArgsTest(string[] args)
        {
            int sum = 0;
            foreach (string arg in args)
            {
                int value = int.Parse(arg);
                sum += value;
            }
            Out.WriteLine(sum);
        }

        public static Wrapper EventTest()
        {
            return Create(args => Main_EventTest(args));
        }
        private static void Main_EventTest(string[] args)
        {
            Console.Error.WriteLine("Standard error should be redirected to a null stream.");
            RaiseEvent(WrapperEventType.Default, new
            {
                Empty = "",
                Message = "'Hello, world!'",
                Math = "1 + 2 + 3 = 6",
                Logic = "A & B & C = D",
                Number = 42
            });
        }
    }
}