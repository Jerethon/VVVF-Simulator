using System;
using System.IO;
using System.Reflection;

namespace VvvfSimulator.Vvvf
{
    public class CustomPwm
    {
        private const int MaxPwmLevel = 2;

        public readonly int SwitchCount = 5;
        public readonly double ModulationIndexDivision = 0.01;
        public readonly uint BlockCount = 0;
        public readonly (double SwitchAngle, int Output)[] SwitchAngleTable = [];
        public readonly bool[] Polarity = [];

        public CustomPwm(Stream? St)
        {
            if (St == null) throw new Exception();

            St.Seek(0, SeekOrigin.Begin);
            byte SwitchCount = (byte)St.ReadByte();

            byte[] DivisionRaw = new byte[8];
            St.Read(DivisionRaw, 0, 8);
            double Division = BitConverter.ToDouble(DivisionRaw, 0);

            byte[] LengthRaw = new byte[4];
            St.Read(LengthRaw, 0, 4);
            uint Length = BitConverter.ToUInt32(LengthRaw, 0);

            this.SwitchCount = SwitchCount;
            this.ModulationIndexDivision = Division;
            this.BlockCount = Length;

            this.SwitchAngleTable = new (double, int)[this.BlockCount * this.SwitchCount];
            this.Polarity = new bool[this.BlockCount];

            for (int i = 0; i < this.BlockCount; i++)
            {
                byte[] PolarityRaw = new byte[1];
                St.Read(PolarityRaw, 0, 1);
                bool Polarity = PolarityRaw[0] == 1;
                this.Polarity[i] = Polarity;

                for (int j = 0; j < this.SwitchCount; j++)
                {
                    byte[] LevelRaw = new byte[1];
                    byte[] SwitchAngleRaw = new byte[8];
                    St.Read(LevelRaw, 0, 1);
                    St.Read(SwitchAngleRaw, 0, 8);
                    int Output = LevelRaw[0];
                    double SwitchAngle = BitConverter.ToDouble(SwitchAngleRaw, 0);
                    this.SwitchAngleTable[i * this.SwitchCount + j] = new(SwitchAngle, Output);
                }
            }

            St.Close();
        }

        public int GetPwm(double M, double X)
        {
            int Index = (int)(M / ModulationIndexDivision);
            if (Index >= this.BlockCount) return 0;

            X %= MyMath.M_2PI;
            int Orthant = (int)(X / MyMath.M_PI_2);
            double Angle = X % MyMath.M_PI_2;

            if ((Orthant & 0x01) == 1)
                Angle = MyMath.M_PI_2 - Angle;

            int Pwm = 0;
            bool Inverted = Polarity[Index];

            for (int i = 0; i < SwitchCount; i++)
            {
                (double SwitchAngle, int Output) = SwitchAngleTable[Index * SwitchCount + i];
                if (SwitchAngle <= Angle) Pwm = Output;
                else break;
            }

            if (Orthant > 1)
                Pwm = MaxPwmLevel - Pwm;

            if(Inverted)
                Pwm = MaxPwmLevel - Pwm;

            return Pwm;
        }

        /// <summary>
        /// Custom Pwm Instances with Preset
        /// </summary>

        public static CustomPwm She3Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She3Default.bin"));
        public static CustomPwm She3Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She3Alt1.bin"));
        public static CustomPwm She9Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She9Default.bin"));
        public static CustomPwm She9Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She9Alt1.bin"));
        public static CustomPwm She9Alt2 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She9Alt2.bin"));
        public static CustomPwm She7Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She7Default.bin"));
        public static CustomPwm She7Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She7Alt1.bin"));
        public static CustomPwm She5Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She5Default.bin"));
        public static CustomPwm She5Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She5Alt1.bin"));
        public static CustomPwm She5Alt2 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She5Alt2.bin"));
        public static CustomPwm She11Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She11Default.bin"));
        public static CustomPwm She11Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She11Alt1.bin"));
        public static CustomPwm She13Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She13Default.bin"));
        public static CustomPwm She13Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She13Alt1.bin"));
        public static CustomPwm She15Default = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She15Default.bin"));
        public static CustomPwm She15Alt1 = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("VvvfSimulator.Vvvf.SwitchAngle.She15Alt1.bin"));
    }
}
