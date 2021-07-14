using System;

namespace ProjektSymulatorProcesora
{
    class ProcessorRegister
    {
        ProcessorsRegister High;
        ProcessorsRegister Low;
        public ProcessorRegister(ushort value = 0)
        {
            High = new();
            Low = new();
            this.setValue(value);
        }
        public ProcessorRegister(ProcessorsRegister high, ProcessorsRegister low)
        {
            High = high;
            Low = low;
        }
        public void setValue(ushort val)
        {
            High.Value = (byte)(val >> 8);
            Low.Value = (byte)(val & 255);
        }
        public ushort getValue()
        {
            byte[] bytes = new byte[] { Low.Value, High.Value };
            return BitConverter.ToUInt16(bytes);
        }
    }
}
