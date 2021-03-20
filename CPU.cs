using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;

namespace SimOS
{
    class CPU
    {
        private int programCounter = 0;
        //private Thread thread;
        private Register register;
        private int regS1, regS2, regD;

        private List<string> instructionList;
        private int opcode = 0;
        private string inputBuffer, outputBuffer;
        private int type, regB, reg1, reg2;
        public string id;
        long address;
        int tempAddress;
        public CPU next;
        string dump = "";
        bool debugMode = false;

        public CPU()
        {
            register = new Register();
            instructionList = new List<string>();
            //thread = new Thread(new ThreadStart(StartWork));
        }

        public CPU(bool debugMode)
        {
            this.debugMode = debugMode;
            register = new Register();
            instructionList = new List<string>();
            //thread = new Thread(new ThreadStart(StartWork));
        }

        public void StartCPU()
        {
            //thread.Start();
        }

        public void StopCPU()
        {
            //thread.Abort();
        }

        public void execute(RAM ram, pcbQueue pcbQueue, Scheduler schedule, cpuQueue que)
        {

            while (pcbQueue.jobsComplete() == false)
            {
                while (this != que.head || pcbQueue.cpuIterator >= pcbQueue.ramIterator)
                {
                    //do nothing
                    if (pcbQueue.jobsComplete() == true)
                    {
                        return;
                    }
                    Thread.Yield();
                }
                PCB temp = pcbQueue.queue[pcbQueue.cpuIterator];
                if (debugMode)
                {
                    Console.WriteLine("CPU " + this.id + " executing job " + temp.processID);
                }
                pcbQueue.cpuIterator++;
                que.deQueue();
                temp.startTime = DateTime.Now.TimeOfDay;
                temp.waitTime = temp.startTime - temp.ramLoadTime;
                PopulateInstructionList(ram, temp);
                this.StartWork(temp);
                temp.timeCompleted = DateTime.Now.TimeOfDay;
                if (debugMode)
                    temp.core = pcbQueue.getCoreDump(temp, ram);
                temp.executionTime = temp.timeCompleted - temp.startTime;
                temp.complete = true;
                this.signal(ram, schedule, temp);
                if (debugMode)
                {
                    Console.WriteLine("Process " + temp.processID + " complete!");
                }
                que.enQueue(this);
                if (debugMode)
                {
                    Console.WriteLine("Jobs complete? " + pcbQueue.jobsComplete());
                }
            }

        }

        public void StartWork(PCB pcb)
        {
            for (int i = 0; i < instructionList.Count; i++)
            {
                type = Utility.BinaryToDecimal(instructionList[i].Substring(0, 2));
                opcode = Utility.BinaryToDecimal(instructionList[i].Substring(2, 6));
                switch (type)
                {
                    //Example Arithmetic Operation that assigns the sum of regS1 and regS2 to regD
                    //00_000101_0000_0001_0010_000000000000 (without the underscores)
                    case 0:
                        //2 bits 0(type) + 6 bits opcode + 4 bits regS1 + 4 bits regS2 + 4 bits regD + 12 bits 0 = 32 bit instruction for Arithmetic
                        if (debugMode)
                        {
                            Console.WriteLine("Performing an ARITHMETIC operation");
                        }
                        regS1 = Utility.BinaryToDecimal(instructionList[i].Substring(8, 4));
                        regS2 = Utility.BinaryToDecimal(instructionList[i].Substring(12, 4));
                        regD = Utility.BinaryToDecimal(instructionList[i].Substring(16, 4));
                        ExecuteOPCode(opcode);
                        break;
                    case 1:
                        //2 bits 01(type) + 6 bits opcode + 4 bits regB + 4 bits regD + 16 bits address = 32 bits for Conditional Branch
                        if (debugMode)
                        {
                            Console.WriteLine("Performing a CONDITIONAL BRANCH operation");
                        }
                        regB = Utility.BinaryToDecimal(instructionList[i].Substring(8, 4));
                        regD = Utility.BinaryToDecimal(instructionList[i].Substring(12, 4));
                        address = Utility.BinaryToDecimal(instructionList[i].Substring(16, 16));
                        ExecuteOPCode(opcode);
                        break;
                    case 2:
                        //2 bits 10(type) + 6 bits opcode + 24 bits address = 32 bits for Uncondition Jump
                        if (debugMode)
                        {
                            Console.WriteLine("Performing an UNCONDITIONAL JUMP operation");
                        }
                        address = Utility.BinaryToDecimal(instructionList[i].Substring(8, 24));
                        ExecuteOPCode(opcode);
                        break;
                    case 3:
                        //2 bits 11(type) + 6 bits opcode + 4 bits reg1 + 4 bits reg2 + 16 bits address = 32 bits for Input/Output
                        if (debugMode)
                        {
                            Console.WriteLine("Performing an I/O operation");
                        }
                        pcb.ioOps++;
                        reg1 = Utility.BinaryToDecimal(instructionList[i].Substring(8, 4));
                        reg2 = Utility.BinaryToDecimal(instructionList[i].Substring(12, 4));
                        address = Utility.BinaryToDecimal(instructionList[i].Substring(16, 16));
                        ExecuteOPCode(opcode);
                        break;
                    default:
                        break;
                }
            }
            instructionList.Clear();
        }

        public void PopulateInstructionList(RAM ram, PCB pcb)
        {
            string instruction = "";
            int lowerBound = pcb.ramBaseAddress;
            int upperBound = lowerBound + (pcb.wordCount * 32);
            int instructionsCount = upperBound / 32 - lowerBound / 32;
            int count = 1;
            if (lowerBound == -1) { return; }
            for (int j = lowerBound + 1; j < upperBound - 1; j++)
            {
                instruction += ram.data[j - 1];
                if (count % 32 == 0)
                {
                    instructionList.Add(instruction);
                    instruction = "";
                }
                count++;
            }
        }


        public void signal(RAM ram, Scheduler schedule, PCB pcb) //Deallocates a jobs memory after completion
        {
            schedule.releaseMem(ram, pcb);
        }

        private void ExecuteOPCode(int oc)
        {
            if (oc < 0 || oc > 26)
            {
                if (debugMode)
                {
                    Console.WriteLine($"OPCODE '{oc}' FOUND NO MATCHING OPERATION.  Aborting...");
                }
                StopCPU();
            }
            else
            {
                if (debugMode)
                {
                    Console.WriteLine($"opcode '{oc}' found! Executing...");
                }
                switch (oc)
                {
                    case 0:
                        //Reads content of input buffer into an accumulator or register
                        if (debugMode)
                        {
                            Console.WriteLine("RD Operation");
                        }
                        if (address > 0)
                        {
                            int tempAddress = (int)address / 4;
                            //We need to set the inputBuffer from the opcode here, possibly some sort of memory access
                            register.AssignValueToRegister(Utility.BinaryToDecimal(inputBuffer), reg1);
                        }
                        else
                        {
                            register.AssignValueToRegister(register.RetrieveValueFromRegister(reg2), reg1);
                        }
                        break;
                    case 1:
                        //Writes the content of the accumulator into output buffer
                        if (debugMode)
                        {
                            Console.WriteLine("WR Operation");
                        }
                        outputBuffer = Utility.DecimalToBinary(register.RetrieveValueFromRegister(reg1));
                        tempAddress = (int)address / 4;
                        break;
                    case 2:
                        //Stores content of register into an address
                        if (debugMode)
                        {
                            Console.WriteLine("ST Operation");
                        }
                        address = register.RetrieveValueFromRegister(regD);
                        break;
                    case 3:
                        //Loads the content of an address into a register
                        if (debugMode)
                        {
                            Console.WriteLine("LW Operation");
                        }
                        register.AssignValueToRegister(register.RetrieveValueFromRegister((int)address), regD);
                        break;
                    case 4:
                        //Transfers content of one register to another register
                        if (debugMode)
                            Console.WriteLine("MOV Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(reg1), reg2);
                        break;
                    case 5:
                        //Adds contents of two regS into the regD
                        if (debugMode)
                            Console.WriteLine("ADD Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(regS1) + register.RetrieveValueFromRegister(regS2), regD);
                        break;
                    case 6:
                        //Subtracts contents of two regS into the regD
                        if (debugMode)
                            Console.WriteLine("SUB Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(regS1) - register.RetrieveValueFromRegister(regS2), regD);
                        break;
                    case 7:
                        //Multiplies contents of two regS into the regD
                        if (debugMode)
                            Console.WriteLine("MUL Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(regS1) * register.RetrieveValueFromRegister(regS2), regD);
                        break;
                    case 8:
                        //Divides contents of two regS into the regD
                        if (debugMode)
                            Console.WriteLine("DIV Operation");
                        if (register.RetrieveValueFromRegister(regS2) == 0)
                        {
                            if (debugMode)
                                Console.WriteLine("Divide by 0 error!");
                        }
                        else
                        {
                            register.AssignValueToRegister(register.RetrieveValueFromRegister(regS1) - register.RetrieveValueFromRegister(regS2), regD);
                        }
                        break;
                    case 9:
                        //Logical AND of two regS into regD
                        if (debugMode)
                            Console.WriteLine("AND Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(reg1) & register.RetrieveValueFromRegister(reg2), regD);
                        break;
                    case 10:
                        //Logical OR of two regS into regD
                        if (debugMode)
                            Console.WriteLine("OR Operation");
                        register.AssignValueToRegister(register.RetrieveValueFromRegister(reg1) | register.RetrieveValueFromRegister(reg2), regD);
                        break;
                    case 11:
                        //Transfers address/data directly into a register
                        if (debugMode)
                            Console.WriteLine("MOVI Operation");
                        tempAddress = (int)address / 4;
                        register.AssignValueToRegister(tempAddress, regD);
                        break;
                    case 12:
                        //Adds a data value directly to the content of a register
                        if (debugMode)
                            Console.WriteLine("ADDI Operation");
                        register.AssignValueToRegister((int)address + register.RetrieveValueFromRegister(regD), regD);
                        break;
                    case 13:
                        //Multiplies a data value directly to the content of a register
                        if (debugMode)
                            Console.WriteLine("MULI Operation");
                        register.AssignValueToRegister((int)address * register.RetrieveValueFromRegister(regD), regD);
                        break;
                    case 14:
                        //Divides a data value directly to the content of a register
                        if (debugMode)
                            Console.WriteLine("DIVI Operation");
                        if (register.RetrieveValueFromRegister(regD) == 0)
                            Console.WriteLine("Divide by 0 error!");
                        else
                            register.AssignValueToRegister((int)address / register.RetrieveValueFromRegister(regD), regD);
                        break;
                    case 15:
                        //Loads a data/address directly into the content of a register
                        if (debugMode)
                            Console.WriteLine("LDI Operation");
                        register.AssignValueToRegister((int)address, regD);
                        break;
                    case 16:
                        //Sets regD to 1 if regS1 is less than regB; else 0
                        if (debugMode)
                            Console.WriteLine("SLT Operation");
                        if (register.RetrieveValueFromRegister(regS1) < register.RetrieveValueFromRegister(regB))
                            register.AssignValueToRegister(1, regD);
                        else
                            register.AssignValueToRegister(0, regD);
                        break;
                    case 17:
                        //Sets regD to 1 if regS1 is less than a data; else 0
                        if (debugMode)
                            Console.WriteLine("SLTI Operation");
                        if (register.RetrieveValueFromRegister(regS1) < (int)address)
                            register.AssignValueToRegister(1, regD);
                        else
                            register.AssignValueToRegister(0, regD);
                        break;
                    case 18:
                        if (debugMode)
                        {
                            Console.WriteLine("HLT Operation");
                            Console.WriteLine("**End of the program**");
                        }
                        StopCPU();
                        programCounter = 0;
                        break;
                    case 19:
                        //Does nothing and moves to next instruction
                        if (debugMode)
                        {
                            Console.WriteLine("NOP Operation");
                            Console.WriteLine("Moving onto the next operation");
                        }
                        break;
                    case 20:
                        //Jumps to a specified location
                        if (debugMode)
                            Console.WriteLine("JMP Operation");
                        programCounter = (int)address / 4;
                        break;
                    case 21:
                        //Branches to an address when content of regB = regD
                        if (debugMode)
                            Console.WriteLine("BEQ Operation");
                        if (register.RetrieveValueFromRegister(regB) == register.RetrieveValueFromRegister(regD))
                        {
                            programCounter = (int)address / 4;
                        }
                        break;
                    case 22:
                        //Branches to an address when content of regB != regD
                        if (debugMode)
                            Console.WriteLine("BNE Operation");
                        if (register.RetrieveValueFromRegister(regB) != register.RetrieveValueFromRegister(regD))
                        {
                            programCounter = (int)address / 4;
                        }
                        break;
                    case 23:
                        //Branches to an address when content of regB = 0
                        if (debugMode)
                            Console.WriteLine("BEZ Operation");
                        if (register.RetrieveValueFromRegister(regB) == 0)
                        {
                            programCounter = (int)address / 4;
                        }
                        break;
                    case 24:
                        //Branches to an address when content of regB != 0
                        if (debugMode)
                            Console.WriteLine("BNZ Operation");
                        if (register.RetrieveValueFromRegister(regB) != 0)
                        {
                            programCounter = (int)address / 4;
                        }
                        break;
                    case 25:
                        //Branches to an address when content of regB > 0
                        if (debugMode)
                            Console.WriteLine("BGZ Operation");
                        if (register.RetrieveValueFromRegister(regB) > 0)
                        {
                            programCounter = (int)address / 4;
                        }
                        break;
                    case 26:
                        //Branches to an address when content of regB < 0
                        if (debugMode)
                            Console.WriteLine("BLZ Operation");
                        if (register.RetrieveValueFromRegister(regB) < 0)
                        {
                            programCounter = (int)address / 4;
                        }
                        break;

                }
            }
        }
    }
}