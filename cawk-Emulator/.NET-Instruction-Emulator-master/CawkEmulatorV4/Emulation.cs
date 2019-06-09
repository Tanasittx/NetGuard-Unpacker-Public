using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CawkEmulatorV4.Instructions.Arithmatic;
using CawkEmulatorV4.Instructions.Arrays;
using CawkEmulatorV4.Instructions.Branches;
using CawkEmulatorV4.Instructions.Conv;
using CawkEmulatorV4.Instructions.Fields;
using CawkEmulatorV4.Instructions.Locals;
using CawkEmulatorV4.Instructions.Misc;
using CawkEmulatorV4.Instructions.Parameters;
using CawkEmulatorV4.Instructions.Pointer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using EasyPredicateKiller;

namespace CawkEmulatorV4
{
    public class Emulation
    {
        private IList<Instruction> _instructionsToEmulate;
        private readonly MethodDef _method;
        public Assembly Assembly;

        public ValueStack ValueStack = new ValueStack();

        public Emulation(MethodDef method)
        {
            _method = method;
            _instructionsToEmulate = method.Body.Instructions;
            SetUp();
        }

        public Emulation(MethodDef method, int start, int end)
        {
            _method = method;
            _instructionsToEmulate = method.Body.Instructions.Skip(start).Take(end - start).ToArray();
            SetUp();
        }

        public ValueStack Emulate2(IList<Instruction> instructions, int start, int end)
        {
            if (_method == null)
                return null;
            _instructionsToEmulate = instructions.Skip(start).Take(end - start).ToArray();
            return Emulate();
        }

        private void SetUp()
        {
            // Set up the event to run the user's action.
            // User never sets the event handler, we do it internally.
            _onInstructionPrepared += (sender, e) =>
            {
                OnInstructionPrepared(this, e);

                // Set the manual reset event, releasing thread.
                _preparedResetEvent.Set();
            };
            _onCallPrepared += (sender, e) =>
            {
                OnCallPrepared(this, e);

                // Set the manual reset event, releasing thread.
                _callPreparedResetEvent.Set();
            };

            MethodDefExt2.OriginalMD = _method.Module as ModuleDefMD;
            foreach (var methodDefParameter in _method.Parameters)
                if (methodDefParameter.Type.IsValueType)
                    ValueStack.Parameters.Add(methodDefParameter, 0);
                else
                    ValueStack.Parameters.Add(methodDefParameter, null);
            ValueStack.Locals = new dynamic[_method.Body.Variables.Count];
            for (var i = 0; i < _method.Body.Variables.Count; i++)
            {
                var bodyVariable = _method.Body.Variables[i];
                if (bodyVariable.Type.IsValueType)
                    ValueStack.Locals[i] = 0;
                else
                    ValueStack.Locals[i] = null;
            }
        }


        public ValueStack Emulate()
        {
            var currentStackVal = ValueStack.CallStack.Count;
            for (var i = 0; i < _instructionsToEmulate.Count; i++)
            {
                //       
                Random rand=  new Random();
                if (i == 162)
                {

                }
                if (i %44==0&&rand.Next(0,12)==2)
                {
                GC.Collect();
                }

                if (i == 22)
                {

                }
                var instruction = _instructionsToEmulate[i];
                var shouldFakeCall = false;
                var bypassCall = false;
                var endMethod = false;
                int pushes;
                int pops;
                instruction.CalculateStackUsage(out pushes, out pops);
                currentStackVal += pushes;

                currentStackVal -= pops;
                
                // Signal that the instruction has been prepared.
                var preparedEvtArgs = new InstructionEventArgs(instruction, pushes, pops);
                _onInstructionPrepared(this, preparedEvtArgs);

                // Wait for handler to finish...
                _preparedResetEvent.Wait();

                if (preparedEvtArgs.Cancel)
                    continue;
                if (preparedEvtArgs.Break) break;

                if (instruction.OpCode.FlowControl == FlowControl.Call)
                {
                    // Signal that the call has been prepared.
                    var callEvtArgs = new CallEventArgs(instruction, pushes, pops);
                    _onCallPrepared(this, callEvtArgs);

                    // Wait for handler to finish...
                    _callPreparedResetEvent.Wait();

                    shouldFakeCall = !callEvtArgs.AllowCall;
                    bypassCall = callEvtArgs.bypassCall;
                    endMethod = callEvtArgs.endMethod;
                }


                if (endMethod)
                    break;
                if (!shouldFakeCall)
                {
                    if (!bypassCall)


                        i = EmulateInstruction(instruction, i);
                }
                else
                {
                    if (!bypassCall)
                    {
                        // Fake the call
                        for (var x = 0; x < pops; x++) ValueStack.CallStack.Pop();
                        for (var x = 0; x < pushes; x++) ValueStack.CallStack.Push(0);
                    }
                }
                if (instruction.OpCode == OpCodes.Leave_S || instruction.OpCode == OpCodes.Leave)
                {
                    currentStackVal = 0;
                }

                //				Console.WriteLine(instruction);
                if (currentStackVal != ValueStack.CallStack.Count)
                    throw new Exception("There has been an issue the stack count is not where it should be");
            }

            return ValueStack;
        }

        private int EmulateInstruction(Instruction instruction, int i)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.UNKNOWN1:
                    throw new NotSupportedException();
                case Code.UNKNOWN2:
                    throw new NotSupportedException();
                case Code.Add:
                    Add.Emulate(ValueStack);
                    break;
                case Code.Add_Ovf:
                case Code.Add_Ovf_Un:
                    Add.Emulate_Ovf(ValueStack);
                    break;
                case Code.And:
                    And.Emulate(ValueStack);
                    break;
                case Code.Arglist:
                    throw new NotSupportedException();
                case Code.Beq:
                case Code.Beq_S:
                    var beqResult = Beq.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return beqResult == -1 ? i : beqResult;
                case Code.Bge:
                case Code.Bge_S:
                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    var bgeResult = Bge.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bgeResult == -1 ? i : bgeResult;
                case Code.Bgt:
                case Code.Bgt_S:
                case Code.Bgt_Un:
                case Code.Bgt_Un_S:
                    var bgtResult = Bgt.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bgtResult == -1 ? i : bgtResult;
                case Code.Ble:
                case Code.Ble_S:
                case Code.Ble_Un:
                case Code.Ble_Un_S:
                    var bleResult = Ble.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bleResult == -1 ? i : bleResult;
                case Code.Blt:
                case Code.Blt_S:
                case Code.Blt_Un:
                case Code.Blt_Un_S:
                    var bltResult = Blt.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bltResult == -1 ? i : bltResult;
                case Code.Bne_Un:
                case Code.Bne_Un_S:
                    var bneResult = Bne.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bneResult == -1 ? i : bneResult;
                case Code.Box:
                    Box.Emulate_Box(ValueStack, instruction);
                    break;
                case Code.Br:
                case Code.Br_S:
                    return Br.Emulate(ValueStack, instruction, _instructionsToEmulate);
                case Code.Break:
                    break;
                case Code.Brfalse:
                case Code.Brfalse_S:
                    var bFalseResult = BrFalse.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bFalseResult == -1 ? i : bFalseResult;
                case Code.Brtrue:
                case Code.Brtrue_S:
                    var bTrueResult = BrTrue.Emulate(ValueStack, instruction, _instructionsToEmulate);
                    return bTrueResult == -1 ? i : bTrueResult;
                case Code.Call:
                    Call.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Calli:
                    throw new NotSupportedException();
                case Code.Callvirt:
                    CallVirt.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Castclass:
                    CastClass.Emulate(ValueStack, instruction);
                    break;
                case Code.Ceq:
                    Ceq.Emulate(ValueStack);
                    break;
                case Code.Cgt:
                case Code.Cgt_Un:
                    Ceq.Emulate(ValueStack);
                    break;
                case Code.Ckfinite:
                    throw new NotSupportedException();
                case Code.Clt:
                case Code.Clt_Un:
                    Clt.Emulate(ValueStack);
                    break;
                case Code.Constrained:
                    throw new NotSupportedException();
                case Code.Conv_I:
                    ConvI.Emulation(ValueStack);
                    break;
                case Code.Conv_I1:
                    throw new NotSupportedException();
                case Code.Conv_I2:
                    ConvI2.Emulation(ValueStack);
                    break;
                case Code.Conv_I4:
                    ConvI4.Emulation(ValueStack);
                    break;
                case Code.Conv_I8:
                    ConvI8.Emulation(ValueStack);
                    break;
                case Code.Conv_Ovf_I:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I1:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I1_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I2:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I2_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I4:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I4_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I8:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_I8_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U1:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U1_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U2:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U2_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U4:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U4_Un:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U8:
                    throw new NotSupportedException();
                case Code.Conv_Ovf_U8_Un:
                    throw new NotSupportedException();
                case Code.Conv_R_Un:
                    throw new NotSupportedException();
                case Code.Conv_R4:
                    throw new NotSupportedException();
                case Code.Conv_R8:
                    throw new NotSupportedException();
                case Code.Conv_U:
                    ConvI.UEmulation(ValueStack);
                    break;
                case Code.Conv_U1:
                    ConvI1.UEmulation(ValueStack);
                    break;
                case Code.Conv_U2:
                    ConvI2.UEmulation(ValueStack);
                    break;
                case Code.Conv_U4:
                    ConvI4.UEmulation(ValueStack);
                    break;
                case Code.Conv_U8:
                    ConvI8.UEmulation(ValueStack);
                    break;
                case Code.Cpblk:
                    throw new NotSupportedException();
                case Code.Cpobj:
                    throw new NotSupportedException();
                case Code.Div:
                    Div.Emulate(ValueStack);
                    break;
                case Code.Div_Un:
                    Div.Emulate_Un(ValueStack);
                    break;
                case Code.Dup:
                    ValueStack.CallStack.Push(ValueStack.CallStack.Peek());
                    break;
                case Code.Endfilter:
                    throw new NotSupportedException();
                case Code.Endfinally:
                    throw new NotSupportedException();
                case Code.Initblk:
                    throw new NotSupportedException();
                case Code.Initobj:
                    ValueStack.CallStack.Pop();
                    break;
                case Code.Isinst:
                    IsInst.Emulate_Box(ValueStack,instruction);
                    break;
                case Code.Jmp:
                    throw new NotSupportedException();
                case Code.Ldarg:
                case Code.Ldarg_0:
                case Code.Ldarg_1:
                case Code.Ldarg_2:
                case Code.Ldarg_3:
                case Code.Ldarg_S:
                    Ldarg.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Ldarga:
                    throw new NotSupportedException();
                case Code.Ldarga_S:
                    throw new NotSupportedException();
                case Code.Ldc_I4:
                case Code.Ldc_I4_0:
                case Code.Ldc_I4_1:
                case Code.Ldc_I4_2:
                case Code.Ldc_I4_3:
                case Code.Ldc_I4_4:
                case Code.Ldc_I4_5:
                case Code.Ldc_I4_6:
                case Code.Ldc_I4_7:
                case Code.Ldc_I4_8:
                    ValueStack.CallStack.Push(instruction.GetLdcI4Value());
                    break;
                case Code.Ldc_I4_M1:
                    ValueStack.CallStack.Push(-1);
                    break;
                case Code.Ldc_I4_S:
                    ValueStack.CallStack.Push((sbyte) instruction.GetLdcI4Value());
                    break;

                case Code.Ldc_I8:
                    ValueStack.CallStack.Push((long) instruction.Operand);
                    break;
                case Code.Ldc_R4:
                    ValueStack.CallStack.Push((float) instruction.GetOperand());
                    break;
                case Code.Ldc_R8:
                    ValueStack.CallStack.Push((double) instruction.GetOperand());
                    break;
                case Code.Ldelem:
                    throw new NotSupportedException();
                case Code.Ldelem_I:
                    throw new NotSupportedException();
                case Code.Ldelem_I1:
                    throw new NotSupportedException();
                case Code.Ldelem_I2:
                    throw new NotSupportedException();
                case Code.Ldelem_I4:
                    throw new NotSupportedException();
                case Code.Ldelem_I8:
                    throw new NotSupportedException();
                case Code.Ldelem_R4:
                    throw new NotSupportedException();
                case Code.Ldelem_R8:
                    throw new NotSupportedException();
                case Code.Ldelem_Ref:
                    LdelemRef.Emulate(ValueStack);
                    break;
                case Code.Ldelem_U1:
                    LdelemI1.UEmulate(ValueStack);
                    break;
                case Code.Ldelem_U2:
                    throw new NotSupportedException();
                case Code.Ldelem_U4:
                    LdelemI4.UEmulate(ValueStack);
                    break;
                case Code.Ldelema:
                    Ldelema.Emulate(ValueStack);
                    break;
                case Code.Ldfld:
                    throw new NotSupportedException();
                case Code.Ldflda:
                    throw new NotSupportedException();
                case Code.Ldftn:
                    throw new NotSupportedException();
                case Code.Ldind_I:
                    throw new NotSupportedException();
                case Code.Ldind_I1:
                    throw new NotSupportedException();
                case Code.Ldind_I2:
                    throw new NotSupportedException();
                case Code.Ldind_I4:
                    LdindI4.Emulate(ValueStack);
                    break;
                case Code.Ldind_I8:
                    throw new NotSupportedException();
                case Code.Ldind_R4:
                    throw new NotSupportedException();
                case Code.Ldind_R8:
                    throw new NotSupportedException();
                case Code.Ldind_Ref:
                    throw new NotSupportedException();
                case Code.Ldind_U1:
                    LdindI1.UEmulate(ValueStack);
                    break;
                case Code.Ldind_U2:
                    LdindI2.UEmulate(ValueStack);
                    break;
                case Code.Ldind_U4:
                    LdindI4.UEmulate(ValueStack);
                    break;
                case Code.Ldlen:
                    Ldlen.Emulate(ValueStack);
                    break;
                case Code.Ldloc:
                case Code.Ldloc_0:
                case Code.Ldloc_1:
                case Code.Ldloc_2:
                case Code.Ldloc_3:
                case Code.Ldloc_S:

                    Ldloc.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Ldloca:
                case Code.Ldloca_S:
                    Ldloc.EmulateLdloca(ValueStack, instruction, _method);
                    break;
                case Code.Ldnull:
                    ValueStack.CallStack.Push(null);
                    break;
                case Code.Ldobj:
                    Ldobj.Emulate(ValueStack, instruction);
                    break;
                case Code.Ldsfld:
                    Ldsfld.Emulate(ValueStack, instruction);
                    break;
                case Code.Ldsflda:
                    throw new NotSupportedException();
                case Code.Ldstr:
                    ValueStack.CallStack.Push(instruction.Operand.ToString());
                    break;
                case Code.Ldtoken:
                    Ldtoken.Emulate(ValueStack, instruction);
                    break;
                case Code.Ldvirtftn:
                    throw new NotSupportedException();
                case Code.Leave:
                    
                case Code.Leave_S:
                    return LEave.Emulate(ValueStack, instruction, _instructionsToEmulate);
                case Code.Localloc:
                    Localloc.Emulate(ValueStack);
                    break;
                case Code.Mkrefany:
                    throw new NotSupportedException();
                case Code.Mul:
                case Code.Mul_Ovf:
                case Code.Mul_Ovf_Un:
                    Mul.Emulate(ValueStack);
                    break;
                case Code.Neg:
                    ValueStack.CallStack.Push(-ValueStack.CallStack.Pop());
                    break;
                case Code.Newarr:
                    NewArr.Emulate(ValueStack, instruction);
                    break;
                case Code.Newobj:
                    throw new NotSupportedException();
                case Code.Nop:
                    break;
                case Code.Not:
                    Not.Emulate(ValueStack);
                    break;
                case Code.Or:
                    Or.Emulate(ValueStack);
                    break;
                case Code.Pop:
                    ValueStack.CallStack.Pop();
                    break;
                case Code.Prefix1:
                    throw new NotSupportedException();
                case Code.Prefix2:
                    throw new NotSupportedException();
                case Code.Prefix3:
                    throw new NotSupportedException();
                case Code.Prefix4:
                    throw new NotSupportedException();
                case Code.Prefix5:
                    throw new NotSupportedException();
                case Code.Prefix6:
                    throw new NotSupportedException();
                case Code.Prefix7:
                    throw new NotSupportedException();
                case Code.Prefixref:
                    throw new NotSupportedException();
                case Code.Readonly:
                    throw new NotSupportedException();
                case Code.Refanytype:
                    throw new NotSupportedException();
                case Code.Refanyval:
                    throw new NotSupportedException();
                case Code.Rem:
                    Rem.Emulate(ValueStack);
                    break;
                case Code.Rem_Un:
                    Rem.Emulate_Un(ValueStack);
                    break;
                case Code.Ret:
                    break;
                case Code.Rethrow:
                    throw new NotSupportedException();
                case Code.Shl:
                    Shl.Emulate(ValueStack);
                    break;
                case Code.Shr:
                    Shr.Emulate(ValueStack);
                    break;
                case Code.Shr_Un:
                    Shr.Emulate_Un(ValueStack);
                    break;
                case Code.Sizeof:
                    throw new NotSupportedException();
                case Code.Starg:
                case Code.Starg_S:
                    Starg.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Stelem:
                    throw new NotSupportedException();
                case Code.Stelem_I:

                    throw new NotSupportedException();
                case Code.Stelem_I1:
                    StelemI1.Emulate(ValueStack);
                    break;
                case Code.Stelem_I2:
                   
                    break;
                case Code.Stelem_I4:
                    Stelem_I4.Emulate(ValueStack);
                    break;
                case Code.Stelem_I8:
                    throw new NotSupportedException();
                case Code.Stelem_R4:
                    throw new NotSupportedException();
                case Code.Stelem_R8:
                    throw new NotSupportedException();
                case Code.Stelem_Ref:
                    Stelem_Ref.Emulate(ValueStack, instruction);
                    break;
                case Code.Stfld:
                    throw new NotSupportedException();
                case Code.Stind_I:
                    throw new NotSupportedException();
                case Code.Stind_I1:
                    StindI1.Emulate(ValueStack);
                    break;
                case Code.Stind_I2:
                    throw new NotSupportedException();
                case Code.Stind_I4:
                    StindI4.Emulate(ValueStack);
                    break;
                case Code.Stind_I8:
                    throw new NotSupportedException();
                case Code.Stind_R4:
                    throw new NotSupportedException();
                case Code.Stind_R8:
                    throw new NotSupportedException();
                case Code.Stind_Ref:
                    throw new NotSupportedException();
                case Code.Stloc:
                case Code.Stloc_0:
                case Code.Stloc_1:
                case Code.Stloc_2:
                case Code.Stloc_3:
                case Code.Stloc_S:
                    Stloc.Emulate(ValueStack, instruction, _method);
                    break;
                case Code.Stobj:
                    Stobj.Emulate(ValueStack, instruction);
                    break;
                case Code.Stsfld:
                    Stsfld.Emulate(ValueStack, instruction);
                    break;
                case Code.Sub:
                    Sub.Emulate(ValueStack);
                    break;
                case Code.Sub_Ovf:
                case Code.Sub_Ovf_Un:
                    Sub.Emulate_Ovf(ValueStack);
                    break;
                case Code.Switch:
                    return Switch.Emulate(ValueStack, instruction, _instructionsToEmulate);
                case Code.Tailcall:
                    throw new NotSupportedException();
                case Code.Throw:
                    throw new NotSupportedException();
                case Code.Unaligned:
                    throw new NotSupportedException();
                case Code.Unbox:
                    throw new NotSupportedException();
                case Code.Unbox_Any:
               //     Box.Emulate_UnBox_Any(ValueStack,instruction);
                    break;
                case Code.Volatile:
                    throw new NotSupportedException();
                case Code.Xor:
                    Xor.Emulate(ValueStack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return i;
        }

        #region Events

        private readonly ManualResetEventSlim _preparedResetEvent = new ManualResetEventSlim();
        private event EventHandler<InstructionEventArgs> _onInstructionPrepared;

        private readonly ManualResetEventSlim _callPreparedResetEvent = new ManualResetEventSlim();
        private event EventHandler<CallEventArgs> _onCallPrepared;

        public Action<Emulation, InstructionEventArgs> OnInstructionPrepared { get; set; } = delegate { };
        public Action<Emulation, CallEventArgs> OnCallPrepared { get; set; } = delegate { };

        #endregion
    }
}