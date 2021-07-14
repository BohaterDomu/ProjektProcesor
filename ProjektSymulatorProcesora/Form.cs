using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjektSymulatorProcesora
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        static Dictionary<string, ProcessorsRegister> sregDict = new()
        {
            { "AH", new ProcessorsRegister() },
            { "AL", new ProcessorsRegister() },
            { "BH", new ProcessorsRegister() },
            { "BL", new ProcessorsRegister() },
            { "CH", new ProcessorsRegister() },
            { "CL", new ProcessorsRegister() },
            { "DH", new ProcessorsRegister() },
            { "DL", new ProcessorsRegister() }
        };

        Dictionary<string, ProcessorRegister> regDict = new()
        {
            { "AX", new ProcessorRegister(sregDict["AH"], sregDict["AL"]) },
            { "BX", new ProcessorRegister(sregDict["BH"], sregDict["BL"]) },
            { "CX", new ProcessorRegister(sregDict["CH"], sregDict["CL"]) },
            { "DX", new ProcessorRegister(sregDict["DH"], sregDict["DL"]) },
            { "SI", new ProcessorRegister() },
            { "DI", new ProcessorRegister() },
            { "BP", new ProcessorRegister() },
            { "SP", new ProcessorRegister(0xFFFE) },
            { "DISP", new ProcessorRegister() }
        };

        static ProcessorMemory memory = new();
        static ProcessorMemory stack = new();

        bool DI_ON = false;
        bool SI_ON = false;
        bool BX_ON = false;
        bool BP_ON = false;
        bool DISP_ON = false;
        private void SI_Checked(object sender, EventArgs e)
        {
            if (SI_ON)
            {
                SI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                SI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void DI_Checked(object sender, EventArgs e)
        {
            if (DI_ON)
            {
                DI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void BX_Checked(object sender, EventArgs e)
        {
            if (BX_ON)
            {
                BX_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BX_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void BP_Checked(object sender, EventArgs e)
        {
            if (BP_ON)
            {
                BP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void DISP_Checked(object sender, EventArgs e)
        {
            if (DISP_ON)
            {
                DISP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DISP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void MOV(object src, object dst)
        {
            if (src is ProcessorRegister && dst is ProcessorRegister)
            {
                ((ProcessorRegister)dst).setValue(((ProcessorRegister)src).getValue());
            }
            else if (src is ProcessorsRegister && dst is ProcessorsRegister)
            {
                ((ProcessorsRegister)dst).Value = ((ProcessorsRegister)src).Value;
            }
            else if (src is ProcessorRegister && dst is ushort)
            {
                memory.setBytes((ushort)dst, ((ProcessorRegister)src).getValue());
            }
            else if (src is ProcessorsRegister && dst is ushort)
            {
                memory.setByte((ushort)dst, ((ProcessorsRegister)src).Value);
            }
            else if (src is ushort && dst is ProcessorRegister)
            {
                ((ProcessorRegister)dst).setValue(memory.getBytes((ushort)src));
            }
            else if (src is ushort && dst is ProcessorsRegister)
            {
                ((ProcessorsRegister)dst).Value = memory.getByte((ushort)src);
            }
        }
        private void XCHG(object src, object dst)
        {
            if (src is ProcessorRegister && dst is ProcessorRegister)
            {
                ushort temp = ((ProcessorRegister)dst).getValue();
                ((ProcessorRegister)dst).setValue(((ProcessorRegister)src).getValue());
                ((ProcessorRegister)src).setValue(temp);
            }
            else if (src is ProcessorsRegister && dst is ProcessorsRegister)
            {
                byte temp = ((ProcessorsRegister)dst).Value;
                ((ProcessorsRegister)dst).Value = ((ProcessorsRegister)src).Value;
                ((ProcessorsRegister)src).Value = temp;
            }
            else if (src is ProcessorRegister && dst is ushort)
            {
                ushort temp = memory.getBytes((ushort)dst);
                memory.setBytes((ushort)dst, ((ProcessorRegister)src).getValue());
                ((ProcessorRegister)src).setValue(temp);
            }
            else if (src is ProcessorsRegister && dst is ushort)
            {
                byte temp = memory.getByte((ushort)dst);
                memory.setByte((ushort)dst, ((ProcessorsRegister)src).Value);
                ((ProcessorsRegister)src).Value = temp;
            }
            else if (src is ushort && dst is ProcessorRegister)
            {
                ushort temp = ((ProcessorRegister)dst).getValue();
                ((ProcessorRegister)dst).setValue(memory.getBytes((ushort)src));
                memory.setBytes((ushort)src, temp);
            }
            else if (src is ushort && dst is ProcessorsRegister)
            {
                byte temp = ((ProcessorsRegister)dst).Value;
                ((ProcessorsRegister)dst).Value = memory.getByte((ushort)src);
                memory.setByte((ushort)src, temp);
            }
        }
        private void PUSH(object src)
        {

            ushort sp = regDict["SP"].getValue();

            if (src is ProcessorRegister) stack.setBytes(sp, ((ProcessorRegister)src).getValue());
            if (src is ushort) stack.setBytes(sp, memory.getBytes((ushort)src));

            regDict["SP"].setValue((ushort)(sp - 2));
        }
        private void POP(object dst)
        {
            ushort sp = ((ushort)(regDict["SP"].getValue() + 2));

            if (dst is ProcessorRegister) ((ProcessorRegister)dst).setValue(stack.getBytes(sp));
            if (dst is ushort) memory.setBytes((ushort)dst, stack.getBytes(sp));

            regDict["SP"].setValue((ushort)(sp));
        }
        private void refresh()
        {
            valueAX.Text = regDict["AX"].getValue().ToString("X4");
            valueBX.Text = regDict["BX"].getValue().ToString("X4");
            valueCX.Text = regDict["CX"].getValue().ToString("X4");
            valueDX.Text = regDict["DX"].getValue().ToString("X4");

            valueSI.Text = regDict["SI"].getValue().ToString("X4");
            valueDI.Text = regDict["DI"].getValue().ToString("X4");
            valueBP.Text = regDict["BP"].getValue().ToString("X4");
            valueSP.Text = regDict["SP"].getValue().ToString("X4");
            valueDISP.Text = regDict["DISP"].getValue().ToString("X4");
        }
        private bool isInputValid(string input)
        {
            return (Regex.IsMatch(input, @"^[a-fA-F0-9]+$") && input.Length <= 4);
        }
        private ushort calculateAddress()
        {
            ushort result = 0;

            if (radioButtonSI.Checked) result += regDict["SI"].getValue();
            if (radioButtonDI.Checked) result += regDict["DI"].getValue();
            if (radioButtonBX.Checked) result += regDict["BX"].getValue();
            if (radioButtonBP.Checked) result += regDict["BP"].getValue();
            if (radioButtonDISP.Checked) result += regDict["DISP"].getValue();

            return result;
        }
        private object[] getOperandsFromText()
        {
            object[] operands = new object[] { comboBoxOPZrodlo.Text, comboBoxOPPrzezn.Text };

            for (int i = 0; i < 2; i++)
            {
                string operand = (string)operands[i];

                if (operand.StartsWith('[') && operand.EndsWith(']'))
                {
                    operand = operand[1..(operand.Length - 1)];
                    if (isInputValid(operand)) operands[i] = Convert.ToUInt16(operand, 16);

                }

                try
                {
                    operands[i] = regDict[operand];
                }
                catch (Exception) { }

                try
                {
                    operands[i] = sregDict[operand];
                }
                catch (Exception) { }

            }

            return operands;
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (isInputValid(boxAX.Text))
                regDict["AX"].setValue(Convert.ToUInt16(boxAX.Text, 16));
            if (isInputValid(boxBX.Text))
                regDict["BX"].setValue(Convert.ToUInt16(boxBX.Text, 16));
            if (isInputValid(boxCX.Text))
                regDict["CX"].setValue(Convert.ToUInt16(boxCX.Text, 16));
            if (isInputValid(boxDX.Text))
                regDict["DX"].setValue(Convert.ToUInt16(boxDX.Text, 16));
            if (isInputValid(boxSI.Text))
                regDict["SI"].setValue(Convert.ToUInt16(boxSI.Text, 16));
            if (isInputValid(boxDI.Text))
                regDict["DI"].setValue(Convert.ToUInt16(boxDI.Text, 16));
            if (isInputValid(boxBP.Text))
                regDict["BP"].setValue(Convert.ToUInt16(boxBP.Text, 16));
            if (isInputValid(boxSP.Text))
                regDict["SP"].setValue(Convert.ToUInt16(boxSP.Text, 16));
            if (isInputValid(boxDISP.Text))
                regDict["DISP"].setValue(Convert.ToUInt16(boxDISP.Text, 16));

            refresh();
        }
        private void buttonDoZrodla_Click(object sender, EventArgs e)
        {
            comboBoxOPZrodlo.Text = $"[{calculateAddress().ToString("X4")}]";
        }
        private void buttonDoPrzezn_Click(object sender, EventArgs e)
        {
            comboBoxOPPrzezn.Text = $"[{calculateAddress().ToString("X4")}]";
        }
        private void buttonMOV_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            MOV(operands[0], operands[1]);
            refresh();
        }
        private void buttonXCHG_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            XCHG(operands[0], operands[1]);
            refresh();
        }
        private void buttonPUSH_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            PUSH(operands[0]);
            refresh();
        }
        private void buttonPOP_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            POP(operands[1]);
            refresh();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void Form_Load(object sender, EventArgs e)
        {
            
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Plus1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void Plus2_Click(object sender, EventArgs e)
        {

        }
    }
}
