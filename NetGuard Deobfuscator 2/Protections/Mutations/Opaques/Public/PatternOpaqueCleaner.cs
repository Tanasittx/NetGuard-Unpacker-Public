using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Opaques.Public
{
    class PatternOpaqueCleaner:MutationsBase
    {
        public static Dictionary<string, Tuple<OpCode[], OpCode[]>> PatternsDictionary =
              new Dictionary<string, Tuple<OpCode[], OpCode[]>>();
        public override bool Deobfuscate()
        {
            if (PatternsDictionary.Count == 0)
                SetUpDict();
            TestReplace();
           return tester();
           
        }

        public static bool TestReplace()
        {
            var modified = false;
            int amount = 0;
            foreach (TypeDef typeDef in ModuleDefMD.GetTypes())
            {
                foreach (MethodDef methods in typeDef.Methods)
                {

                    if (!methods.HasBody) continue;
                    if (methods.MDToken.ToInt32() != 0x06000010)
                    {
                        //  continue;
                    }

                    foreach (KeyValuePair<string, Tuple<OpCode[], OpCode[]>> keyValuePair in PatternsDictionary)
                    {
                        var patternMatchj = FindOpCodePatterns(methods.Body.Instructions, keyValuePair.Value.Item1);
                        amount += patternMatchj.Count;
                        foreach (Instruction[] instructionse in patternMatchj)
                        {
                            for (int i = 0; i < instructionse.Length; i++)
                            {
                                if (instructionse[i].IsLdloc())
                                {

                                    Local loc = instructionse[i].GetLocal(methods.Body.Variables);
                                    if (methods.MDToken.ToInt32() == 0x06000014)
                                    {
                                        if (loc.Index == 2)
                                        {

                                        }
                                //        Console.WriteLine(loc.Type.FullName);

                                    }
                                    instructionse[i].OpCode = OpCodes.Ldc_R8;
                                    instructionse[i].Operand = (double)(10);
                                    modified = true;
                                }
                            }
                        }

                    }
                }
            }


            return modified;
            //     Console.ReadLine();

        }
        public static IList<Instruction[]> FindOpCodePatterns(IList<Instruction> instructions, IList<OpCode> pattern)
        {
            List<Instruction[]> list = new List<Instruction[]>();

            for (Int32 i = 0; i < instructions.Count; i++)
            {
                List<Instruction> current = new List<Instruction>();

                for (Int32 j = i, k = 0; j < instructions.Count && k < pattern.Count; j++, k++)
                {
                    if (instructions[j].OpCode != pattern[k])
                        break;
                    else current.Add(instructions[j]);
                }

                if (current.Count == pattern.Count)
                    list.Add(current.ToArray());
            }

            return list;
        }

        public static void SetUpDict()
        {

            OpCode[] pattern1 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Add, OpCodes.Ldloc,
                OpCodes.Conv_R8, OpCodes.Call
            };
            OpCode[] ReplaceWith =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern1", new Tuple<OpCode[], OpCode[]>(pattern1, ReplaceWith));
            OpCode[] pattern2 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Call, OpCodes.Add, OpCodes.Ldloc, OpCodes.Conv_R8,
                OpCodes.Call
            };
            OpCode[] ReplaceWit2h =
                {OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_8, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop};
            PatternsDictionary.Add("Pattern2", new Tuple<OpCode[], OpCode[]>(pattern2, ReplaceWit2h));
            OpCode[] pattern3 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8,
                OpCodes.Call, OpCodes.Add
            };
            OpCode[] ReplaceWit3h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_8, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern3", new Tuple<OpCode[], OpCode[]>(pattern3, ReplaceWit3h));
            /*
             * 454	069F	ldc.r8	10
               455	06A8	ldloc	V_15 (15)
               456	06AC	conv.r8
               457	06AD	ldc.r8	4
               458	06B6	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               459	06BB	add
               460	06BC	ldloc	V_15 (15)
               461	06C0	conv.r8
               462	06C1	ldc.r8	4
               463	06CA	mul
               464	06CB	add
               465	06CC	ldc.r8	4
               466	06D5	ldloc	V_15 (15)
               467	06D9	conv.r8
               468	06DA	mul
               469	06DB	bge.s	472 (06E4) ldc.i4 1
               
             */
            OpCode[] pattern4 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Call, OpCodes.Add,
                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Mul, OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Ldloc,
                OpCodes.Conv_R8, OpCodes.Mul,
            };
            OpCode[] ReplaceWit4h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
            };
            PatternsDictionary.Add("Pattern4", new Tuple<OpCode[], OpCode[]>(pattern4, ReplaceWit4h));
            /*
             * 33	0072	ldc.r8	3
              34	007B	ldc.r8	4
              35	0084	ldloc	V_0 (0)
              36	0088	conv.r8
              37	0089	ldc.r8	2
              38	0092	mul
              39	0093	call	float64 [mscorlib]System.Math::Sin(float64)
              40	0098	add
              41	0099	bge.s	44 (00A2) ldc.i4 0x23D7E566
              
             */
            OpCode[] pattern5 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Mul,
                OpCodes.Call, OpCodes.Add,
            };
            OpCode[] ReplaceWit5h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_4, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern5", new Tuple<OpCode[], OpCode[]>(pattern5, ReplaceWit5h));
            /*
             * 242	03B4	ldc.r8	6
               243	03BD	ldloc	V_25 (25)
               244	03C1	conv.r8
               245	03C2	mul
               246	03C3	ldc.r8	10
               247	03CC	ldloc	V_25 (25)
               248	03D0	conv.r8
               249	03D1	ldc.r8	4
               250	03DA	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               251	03DF	add
               252	03E0	ldloc	V_25 (25)
               253	03E4	conv.r8
               254	03E5	ldc.r8	6
               255	03EE	mul
               256	03EF	add
               257	03F0	bge.s	260 (03F9) ldc.i4 0xC1FB318
               
             */
            OpCode[] pattern6 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Ldc_R8, OpCodes.Ldloc,
                OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Call, OpCodes.Add, OpCodes.Ldloc, OpCodes.Conv_R8,
                OpCodes.Ldc_R8, OpCodes.Mul, OpCodes.Add,
            };
            OpCode[] ReplaceWit6h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern6", new Tuple<OpCode[], OpCode[]>(pattern6, ReplaceWit6h));
            /*
             * 316	04F2	ldc.r8	3
               317	04FB	ldloc	V_1 (1)
               318	04FF	conv.r8
               319	0500	call	float64 [mscorlib]System.Math::Cos(float64)
               320	0505	add
               321	0506	ldloc	V_1 (1)
               322	050A	conv.r8
               323	050B	call	float64 [mscorlib]System.Math::Sin(float64)
               324	0510	bge.s	327 (0519) ldc.i4 0x17
               
             */
            OpCode[] pattern7 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Add, OpCodes.Ldloc,
                OpCodes.Conv_R8, OpCodes.Call,
            };
            OpCode[] ReplaceWit7h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern7", new Tuple<OpCode[], OpCode[]>(pattern7, ReplaceWit7h));
            /*
             * 199	0319	ldc.r8	5
               200	0322	ldloc	V_10 (10)
               201	0326	conv.r8
               202	0327	ldc.r8	2
               203	0330	mul
               204	0331	call	float64 [mscorlib]System.Math::Sin(float64)
               205	0336	add
               206	0337	ldc.r8	4
               207	0340	bge.s	210 (0349) ldc.i4 12
               
             */
            OpCode[] pattern8 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Mul, OpCodes.Call,
                OpCodes.Add, OpCodes.Ldc_R8,
            };
            OpCode[] ReplaceWit8h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern8", new Tuple<OpCode[], OpCode[]>(pattern8, ReplaceWit8h));
            /*
             * 189	030D	ldloc	V_0 (0)
               190	0311	conv.r8
               191	0312	ldloc	V_0 (0)
               192	0316	conv.r8
               193	0317	ldc.r8	2
               194	0320	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               195	0325	add
               196	0326	ldloc	V_0 (0)
               197	032A	conv.r8
               198	032B	call	float64 [mscorlib]System.Math::Exp(float64)
               199	0330	bge.s	202 (0339) ldc.i4 0x2D4CEDDE
               

             */
            OpCode[] pattern9 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8,
                OpCodes.Call, OpCodes.Add, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call,
            };
            OpCode[] ReplaceWit9h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern9", new Tuple<OpCode[], OpCode[]>(pattern9, ReplaceWit9h));
            /*
             * 292	03ED	ldloc	V_3 (3)
               293	03F1	conv.r8
               294	03F2	call	float64 [mscorlib]System.Math::Exp(float64)
               295	03F7	ldloc	V_3 (3)
               296	03FB	conv.r8
               297	03FC	ldloc	V_3 (3)
               298	0400	conv.r8
               299	0401	ldc.r8	2
               300	040A	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               301	040F	add
               302	0410	bge.s	305 (0419) ldc.i4 -0x10505C5F
               
             */
            OpCode[] pattern10 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldloc, OpCodes.Conv_R8,
                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8, OpCodes.Call, OpCodes.Add,
            };
            OpCode[] ReplaceWit10h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern10", new Tuple<OpCode[], OpCode[]>(pattern10, ReplaceWit10h));
            /*
             * 163	0232	ldloc	V_17 (17)
164	0236	conv.r8
165	0237	ldloc	V_17 (17)
166	023B	conv.r8
167	023C	ldc.r8	2
168	0245	call	float64 [mscorlib]System.Math::Pow(float64, float64)
169	024A	add
170	024B	ldloc	V_17 (17)
171	024F	conv.r8
172	0250	call	float64 [mscorlib]System.Math::Exp(float64)
173	0255	bge.s	176 (025E) ldc.i4 0x75074DC9

             */
            OpCode[] pattern11 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Ldc_R8,
                OpCodes.Call, OpCodes.Add, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call,
            };
            OpCode[] ReplaceWit11h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern11", new Tuple<OpCode[], OpCode[]>(pattern11, ReplaceWit11h));
            /*
275	049D	ldloc	V_17 (17)
               276	04A1	call	float64 [mscorlib]System.Math::Abs(float64)
               277	04A6	ldc.r4	0
               278	04AB	add
               279	04AC	ldc.r8	1
               280	04B5	add
               281	04B6	ldc.r8	0
               282	04BF	ldc.r4	0
               283	04C4	stloc	V_17 (17)
               284	04C8	bge.s	287 (04D1) ldc.i4 -0x1F70216A
               
               
             */
            OpCode[] pattern12 =
            {

                OpCodes.Ldloc, OpCodes.Call, OpCodes.Ldc_R4, OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Add,
                OpCodes.Ldc_R8, OpCodes.Ldc_R4, OpCodes.Stloc,
            };
            OpCode[] ReplaceWit12h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern12", new Tuple<OpCode[], OpCode[]>(pattern12, ReplaceWit12h));
            /*
             * 326	056D	ldloc	V_17 (17)
327	0571	call	float64 [mscorlib]System.Math::Abs(float64)
328	0576	ldc.r4	0
329	057B	add
330	057C	ldc.r8	1
331	0585	add
332	0586	ldc.r4	0
333	058B	ldloc	V_17 (17)
334	058F	add
335	0590	call	float64 [mscorlib]System.Math::Abs(float64)
336	0595	dup
337	0596	conv.r8
338	0597	stloc	V_17 (17)
339	059B	bge.s	342 (05A4) ldc.i4 0

             */
            OpCode[] pattern13 =
            {

                OpCodes.Ldloc, OpCodes.Call, OpCodes.Ldc_R4, OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Add,
                OpCodes.Ldc_R4, OpCodes.Ldloc, OpCodes.Add,OpCodes.Call,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit13h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern13", new Tuple<OpCode[], OpCode[]>(pattern13, ReplaceWit13h));
            /*
             * 742	0C83	ldc.r4	0
               743	0C88	ldloc	V_17 (17)
               744	0C8C	add
               745	0C8D	call	float64 [mscorlib]System.Math::Abs(float64)
               746	0C92	ldloc	V_17 (17)
               747	0C96	call	float64 [mscorlib]System.Math::Abs(float64)
               748	0C9B	ldc.r4	0
               749	0CA0	add
               750	0CA1	ldc.r8	1
               751	0CAA	add
               752	0CAB	dup
               753	0CAC	conv.r8
               754	0CAD	stloc	V_17 (17)
               755	0CB1	bge.s	758 (0CBA) ldc.i4 0x31FE4C9
               
             */
            OpCode[] pattern14 =
            {

                OpCodes.Ldc_R4, OpCodes.Ldloc, OpCodes.Add, OpCodes.Call, OpCodes.Ldloc, OpCodes.Call,
                OpCodes.Ldc_R4, OpCodes.Add, OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit14h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern14", new Tuple<OpCode[], OpCode[]>(pattern14, ReplaceWit14h));
            /*
11	0031	ldloc	V_1 (1)
12	0035	call	float64 [mscorlib]System.Math::Abs(float64)
13	003A	ldc.r4	0
14	003F	add
15	0040	ldc.r8	1
16	0049	add
17	004A	ldc.r8	0
18	0053	ldc.r4	0
19	0058	stloc	V_1 (1)
20	005C	bge.s	23 (0065) ldc.i4 0x70E18820

               
             */
            OpCode[] pattern15 =
            {

                OpCodes.Ldloc, OpCodes.Call, OpCodes.Ldc_R4, OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Add,
                OpCodes.Ldc_R8, OpCodes.Ldc_R4, OpCodes.Stloc
            };
            OpCode[] ReplaceWit15h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern15", new Tuple<OpCode[], OpCode[]>(pattern15, ReplaceWit15h));
            /*
             * 796	0D77	ldloc	V_17 (17)
               797	0D7B	call	float64 [mscorlib]System.Math::Abs(float64)
               798	0D80	ldc.r4	0
               799	0D85	add
               800	0D86	ldc.r8	1
               801	0D8F	add
               802	0D90	dup
               803	0D91	conv.r8
               804	0D92	stloc	V_17 (17)
               805	0D96	bge.s	808 (0D9F) ldc.i4 0x629A9CFC
               
             */
            OpCode[] pattern16 =
            {

                OpCodes.Ldloc, OpCodes.Call, OpCodes.Ldc_R4, OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Add,
                OpCodes.Dup, OpCodes.Conv_R8, OpCodes.Stloc
            };
            OpCode[] ReplaceWit16h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern16", new Tuple<OpCode[], OpCode[]>(pattern16, ReplaceWit16h));
            /*#
             40	00A6	ldc.r8	1
               41	00AF	ldloc	V_0 (0)
               42	00B3	conv.r8
               43	00B4	mul
               44	00B5	call	float64 [mscorlib]System.Math::Sin(float64)
               45	00BA	ldc.r8	2
               46	00C3	add
               47	00C4	dup
               48	00C5	conv.r8
               49	00C6	stloc	V_6 (6)
               50	00CA	bge.s	53 (00D3) ldc.i4 0x3EDFEFC5
               
             
             */
            OpCode[] pattern17 =
            {

                OpCodes.Ldc_R8,OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Add,
                OpCodes.Dup, OpCodes.Conv_R8, OpCodes.Stloc
            };
            OpCode[] ReplaceWit17h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern17", new Tuple<OpCode[], OpCode[]>(pattern17, ReplaceWit17h));
            /*
             * 18	0051	ldloc	V_6 (6)
               19	0055	call	float64 [mscorlib]System.Math::Abs(float64)
               20	005A	ldc.r8	3
               21	0063	ldc.r4	0
               22	0068	call	float64 [mscorlib]System.Math::Abs(float64)
               23	006D	mul
               24	006E	add
               25	006F	ldc.r8	1
               26	0078	add
               27	0079	dup
               28	007A	conv.r8
               29	007B	stloc	V_6 (6)
               30	007F	bge.s	33 (0088) ldc.i4 0x1DF7ADF0
               
             */
            OpCode[] pattern18 =
            {

                OpCodes.Ldloc,OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Ldc_R4, OpCodes.Call, OpCodes.Mul, OpCodes.Add,
                OpCodes.Ldc_R8, OpCodes.Add, OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit18h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern18", new Tuple<OpCode[], OpCode[]>(pattern18, ReplaceWit18h));
            /*
                      * 40	009C	ldc.r8	4
                        41	00A5	ldloc	V_0 (0)
                        42	00A9	conv.r8
                        43	00AA	call	float64 [mscorlib]System.Math::Abs(float64)
                        44	00AF	ldc.r8	2
                        45	00B8	call	float64 [mscorlib]System.Math::Pow(float64, float64)
                        46	00BD	mul
                        47	00BE	ldc.r8	10
                        48	00C7	add
                        49	00C8	ldc.r8	4
                        50	00D1	ldloc	V_0 (0)
                        51	00D5	conv.r8
                        52	00D6	mul
                        53	00D7	dup
                        54	00D8	conv.r8
                        55	00D9	pop
                        56	00DA	bge.s	59 (00E3) ldc.i4 0x40

                      */
            OpCode[] pattern19 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Call,
                OpCodes.Mul,OpCodes.Ldc_R8, OpCodes.Add, OpCodes.Ldc_R8,OpCodes.Ldloc,OpCodes.Conv_R8,OpCodes.Mul,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Pop
            };
            OpCode[] ReplaceWit19h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern19", new Tuple<OpCode[], OpCode[]>(pattern19, ReplaceWit19h));
            /*
             * 191	0302	ldloc	V_6 (6)
               192	0306	call	float64 [mscorlib]System.Math::Abs(float64)
               193	030B	ldc.r8	4
               194	0314	ldloc	V_0 (0)
               195	0318	conv.r8
               196	0319	call	float64 [mscorlib]System.Math::Abs(float64)
               197	031E	mul
               198	031F	add
               199	0320	ldc.r8	1
               200	0329	add
               201	032A	ldloc	V_0 (0)
               202	032E	conv.r8
               203	032F	ldloc	V_6 (6)
               204	0333	add
               205	0334	call	float64 [mscorlib]System.Math::Abs(float64)
               206	0339	dup
               207	033A	conv.r8
               208	033B	stloc	V_6 (6)
               209	033F	bge.s	212 (0348) ldc.i4 1
               
             */
            OpCode[] pattern20 =
            {

                OpCodes.Ldloc, OpCodes.Call,OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call,
                OpCodes.Mul,OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Add,OpCodes.Ldloc,OpCodes.Conv_R8,OpCodes.Ldloc,OpCodes.Add,OpCodes.Call,OpCodes.Dup,
                OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit20h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern20", new Tuple<OpCode[], OpCode[]>(pattern20, ReplaceWit20h));
            /*
             * 23	006F	ldloc	V_0 (0)
               24	0073	conv.r8
               25	0074	ldloc	V_6 (6)
               26	0078	add
               27	0079	call	float64 [mscorlib]System.Math::Abs(float64)
               28	007E	ldloc	V_6 (6)
               29	0082	call	float64 [mscorlib]System.Math::Abs(float64)
               30	0087	ldc.r8	3
               31	0090	ldloc	V_0 (0)
               32	0094	conv.r8
               33	0095	call	float64 [mscorlib]System.Math::Abs(float64)
               34	009A	mul
               35	009B	add
               36	009C	ldc.r8	1
               37	00A5	add
               38	00A6	dup
               39	00A7	conv.r8
               40	00A8	stloc	V_6 (6)
               41	00AC	bge	44 (00BB) ldc.i4 0x71404CE8
               
             */
            OpCode[] pattern21 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8,OpCodes.Ldloc, OpCodes.Add, OpCodes.Call, OpCodes.Ldloc,
                OpCodes.Call,OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8,OpCodes.Call,OpCodes.Mul,OpCodes.Add,OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Dup,
                OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit21h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern21", new Tuple<OpCode[], OpCode[]>(pattern21, ReplaceWit21h));
            /*
             * 334	0584	ldc.r8	5
               335	058D	ldloc	V_19 (19)
               336	0591	conv.r8
               337	0592	mul
               338	0593	ldc.r8	5
               339	059C	ldloc	V_19 (19)
               340	05A0	conv.r8
               341	05A1	call	float64 [mscorlib]System.Math::Abs(float64)
               342	05A6	ldc.r8	2
               343	05AF	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               344	05B4	mul
               345	05B5	ldc.r8	10
               346	05BE	add
               347	05BF	bge	350 (05CE) ldc.i4 0x14425609
               
             */
            OpCode[] pattern22 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Ldc_R8, OpCodes.Ldloc,
                OpCodes.Conv_R8,OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Call,OpCodes.Mul,OpCodes.Ldc_R8,OpCodes.Add
            };
            OpCode[] ReplaceWit22h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern22", new Tuple<OpCode[], OpCode[]>(pattern22, ReplaceWit22h));
            /*
             * 229	03CF	ldloc	V_3 (3)
230	03D3	conv.r8
231	03D4	call	float64 [mscorlib]System.Math::Cos(float64)
232	03D9	ldc.r8	3
233	03E2	add
234	03E3	ldloc	V_3 (3)
235	03E7	conv.r8
236	03E8	call	float64 [mscorlib]System.Math::Sin(float64)
237	03ED	dup
238	03EE	conv.r8
239	03EF	stloc	V_18 (18)
240	03F3	bge	243 (0402) ldc.i4 1

             */
            OpCode[] pattern23 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8,OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Add, OpCodes.Ldloc,
                OpCodes.Conv_R8,OpCodes.Call, OpCodes.Dup, OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit23h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern23", new Tuple<OpCode[], OpCode[]>(pattern23, ReplaceWit23h));
            /*
             * 121	0200	ldloc	V_11 (11)
               122	0204	conv.r8
               123	0205	call	float64 [mscorlib]System.Math::Sin(float64)
               124	020A	ldloc	V_11 (11)
               125	020E	conv.r8
               126	020F	call	float64 [mscorlib]System.Math::Cos(float64)
               127	0214	ldc.r8	3
               128	021D	add
               129	021E	dup
               130	021F	conv.r8
               131	0220	stloc	V_18 (18)
               132	0224	bge	135 (0233) ldc.i4 0x7D62C870
               
             */
            OpCode[] pattern24 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8,OpCodes.Call, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call,
                OpCodes.Ldc_R8,OpCodes.Add, OpCodes.Dup, OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit24h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern24", new Tuple<OpCode[], OpCode[]>(pattern24, ReplaceWit24h));
            /*
             * 684	0B87	ldc.r8	2
               685	0B90	ldloc	V_16 (16)
               686	0B94	conv.r8
               687	0B95	call	float64 [mscorlib]System.Math::Abs(float64)
               688	0B9A	ldc.r8	2
               689	0BA3	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               690	0BA8	mul
               691	0BA9	ldc.r8	10
               692	0BB2	add
               693	0BB3	ldc.r8	2
               694	0BBC	ldloc	V_16 (16)
               695	0BC0	conv.r8
               696	0BC1	mul
               697	0BC2	dup
               698	0BC3	conv.r8
               699	0BC4	stloc	V_19 (19)
               700	0BC8	bge	703 (0BD7) ldc.i4 0x4AED0D0E
               
             */
            OpCode[] pattern25 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldc_R8, OpCodes.Call,
                OpCodes.Mul,OpCodes.Ldc_R8, OpCodes.Add, OpCodes.Ldc_R8,OpCodes.Ldloc,OpCodes.Conv_R8,OpCodes.Mul,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit25h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern25", new Tuple<OpCode[], OpCode[]>(pattern25, ReplaceWit25h));
            /*
             * 581	09D2	ldloc	V_16 (16)
               582	09D6	ldc.r8	1
               583	09DF	ldloc	V_3 (3)
               584	09E3	conv.r8
               585	09E4	mul
               586	09E5	call	float64 [mscorlib]System.Math::Sin(float64)
               587	09EA	ldc.r8	2
               588	09F3	add
               589	09F4	ldc.r8	1
               590	09FD	ldc.r4	1
               591	0A02	stloc	V_19 (19)
               592	0A06	bge	595 (0A15) ldc.i4 1
               
             */
            OpCode[] pattern26 =
            {

                 OpCodes.Ldc_R8,OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Call,
                OpCodes.Ldc_R8,OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Ldc_R4,OpCodes.Stloc
            };
            OpCode[] ReplaceWit26h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern26", new Tuple<OpCode[], OpCode[]>(pattern26, ReplaceWit26h));
            /*
             * 626	0AAF	ldc.r8	2
627	0AB8	ldloc	V_10 (10)
628	0ABC	conv.r8
629	0ABD	mul
630	0ABE	call	float64 [mscorlib]System.Math::Sin(float64)
631	0AC3	ldc.r8	6
632	0ACC	add
633	0ACD	ldc.r8	5
634	0AD6	ldc.r4	5
635	0ADB	stloc	V_19 (19)
636	0ADF	bge	639 (0AEE) ldc.i4 0

             */
            OpCode[] pattern27 =
            {

                OpCodes.Ldc_R8, OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Call, OpCodes.Ldc_R8,
                OpCodes.Add, OpCodes.Ldc_R8, OpCodes.Ldc_R4,OpCodes.Stloc
            };
            OpCode[] ReplaceWit27h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern27", new Tuple<OpCode[], OpCode[]>(pattern27, ReplaceWit27h));
            /*
             * 175	0267	ldc.r8	2
               176	0270	ldloc	V_3 (3)
               177	0274	conv.r8
               178	0275	mul
               179	0276	call	float64 [mscorlib]System.Math::Sin(float64)
               180	027B	ldc.r8	2
               181	0284	add
               182	0285	ldc.r8	1
               183	028E	bge.s	186 (0297) ldc.i4 -0x1F70216E
               
             */
            OpCode[] pattern28 =
            {

                 OpCodes.Ldc_R8,OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Mul, OpCodes.Call,
                OpCodes.Ldc_R8,OpCodes.Add, OpCodes.Ldc_R8
            };
            OpCode[] ReplaceWit28h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern28", new Tuple<OpCode[], OpCode[]>(pattern28, ReplaceWit28h));
            /*
             * 21	0054	ldc.r4	0
               22	0059	ldloc	V_18 (18)
               23	005D	add
               24	005E	call	float64 [mscorlib]System.Math::Abs(float64)
               25	0063	ldloc	V_18 (18)
               26	0067	call	float64 [mscorlib]System.Math::Abs(float64)
               27	006C	ldc.r8	3
               28	0075	ldc.r8	0
               29	007E	nop
               30	007F	mul
               31	0080	add
               32	0081	ldc.r8	1
               33	008A	add
               34	008B	dup
               35	008C	conv.r8
               36	008D	stloc	V_18 (18)
               37	0091	bge	40 (00A0) ldc.i4 0x1BCACDE3
               
             */
            OpCode[] pattern29 =
            {

                OpCodes.Ldc_R4,OpCodes.Ldloc, OpCodes.Add, OpCodes.Call, OpCodes.Ldloc,
                OpCodes.Call,OpCodes.Ldc_R8, OpCodes.Ldc_R8,OpCodes.Nop,OpCodes.Mul,OpCodes.Add,OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc
            };
            OpCode[] ReplaceWit29h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern29", new Tuple<OpCode[], OpCode[]>(pattern29, ReplaceWit29h));
            /*
             * 79	0120	ldloc	V_2 (2)
               80	0124	ldloc	V_2 (2)
               81	0128	conv.r8
               82	0129	call	float64 [mscorlib]System.Math::Sin(float64)
               83	012E	ldloc	V_2 (2)
               84	0132	conv.r8
               85	0133	call	float64 [mscorlib]System.Math::Cos(float64)
               86	0138	ldc.r8	3
               87	0141	add
               88	0142	bge.s	91 (014B) ldc.i4 0x38DFE14B
               
             */
            OpCode[] pattern30 =
            {

                OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldloc,
                OpCodes.Conv_R8,OpCodes.Call, OpCodes.Ldc_R8,OpCodes.Add
            };
            OpCode[] ReplaceWit30h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern30", new Tuple<OpCode[], OpCode[]>(pattern30, ReplaceWit30h));
            /*4
             187	02A5	ldc.r8	1
               188	02AE	ldloc	V_3 (3)
               189	02B2	conv.r8
               190	02B3	call	float64 [mscorlib]System.Math::Abs(float64)
               191	02B8	ldc.r8	2
               192	02C1	call	float64 [mscorlib]System.Math::Pow(float64, float64)
               193	02C6	mul
               194	02C7	ldc.r8	10
               195	02D0	add
               196	02D1	ldc.r8	1
               197	02DA	ldloc	V_3 (3)
               198	02DE	conv.r8
               199	02DF	mul
               200	02E0	bge.s	203 (02E9) ldc.i4 0x7ADD23A2
               */
            OpCode[] pattern31 =
            {

                OpCodes.Ldc_R8,OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call, OpCodes.Ldc_R8,
                OpCodes.Call,OpCodes.Mul, OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Ldc_R8,OpCodes.Ldloc,OpCodes.Conv_R8,OpCodes.Mul
            };
            OpCode[] ReplaceWit31h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern31", new Tuple<OpCode[], OpCode[]>(pattern31, ReplaceWit31h));
            /*
             * 197	02BB	ldc.r8	2
               198	02C4	ldc.r8	1
               199	02CD	ldloc	V_11 (11)
               200	02D1	conv.r8
               201	02D2	mul
               202	02D3	call	float64 [mscorlib]System.Math::Sin(float64)
               203	02D8	ldc.r8	3
               204	02E1	add
               205	02E2	bge.s	208 (02EB) ldc.i4 0x20EE930A
               
             */
            OpCode[] pattern32 =
            {

                OpCodes.Ldc_R8,OpCodes.Ldc_R8, OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Mul,
                OpCodes.Call,OpCodes.Ldc_R8, OpCodes.Add,
            };
            OpCode[] ReplaceWit32h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern32", new Tuple<OpCode[], OpCode[]>(pattern32, ReplaceWit32h));
            /*
             * 185	0292	ldloc	V_10 (10)
               186	0296	conv.r8
               187	0297	call	float64 [mscorlib]System.Math::Cos(float64)
               188	029C	ldc.r8	3
               189	02A5	add
               190	02A6	ldloc	V_10 (10)
               191	02AA	conv.r8
               192	02AB	call	float64 [mscorlib]System.Math::Sin(float64)
               193	02B0	bge.s	196 (02B9) ldc.i4 -0x1F70216C
               
             */
            OpCode[] pattern33 =
            {

                 OpCodes.Ldloc, OpCodes.Conv_R8, OpCodes.Call,
                OpCodes.Ldc_R8,OpCodes.Add, OpCodes.Ldloc,OpCodes.Conv_R8,OpCodes.Call
            };
            OpCode[] ReplaceWit33h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern33", new Tuple<OpCode[], OpCode[]>(pattern33, ReplaceWit33h));
            /*
             * 183	0290	conv.r8
               184	0291	call	float64 [mscorlib]System.Math::Sin(float64)
               185	0296	ldloc	V_17 (17)
               186	029A	conv.r8
               187	029B	call	float64 [mscorlib]System.Math::Cos(float64)
               188	02A0	ldc.r8	3
               189	02A9	add
               190	02AA	bge.s	193 (02B3) ldc.i4 0x330670A8
               
             */
            OpCode[] pattern34 =
            {
 OpCodes.Conv_R8, OpCodes.Call,OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Call,OpCodes.Ldc_R8,OpCodes.Add
            };
            OpCode[] ReplaceWit34h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern34", new Tuple<OpCode[], OpCode[]>(pattern34, ReplaceWit34h));
            /*
       580	0A1F	ldloc	V_17 (17)
               581	0A23	conv.r8
               582	0A24	call	float64 [mscorlib]System.Math::Sin(float64)
               583	0A29	ldc.r8	10
               584	0A32	conv.r8
               585	0A33	call	float64 [mscorlib]System.Math::Cos(float64)
               586	0A38	ldc.r8	3
               587	0A41	add
               588	0A42	bge	591 (0A51) ldc.i4 0x330670A8
               
             */
            OpCode[] pattern35 =
            {
                OpCodes.Ldloc,OpCodes.Conv_R8, OpCodes.Call,OpCodes.Ldc_R8,OpCodes.Conv_R8,OpCodes.Call,OpCodes.Ldc_R8,OpCodes.Add
            };
            OpCode[] ReplaceWit35h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern35", new Tuple<OpCode[], OpCode[]>(pattern35, ReplaceWit35h));
            /*
             * 7	001D	ldloc	V_5 (5)
               8	0021	call	float64 [mscorlib]System.Math::Abs(float64)
               9	0026	ldc.r4	0
               10	002B	add
               11	002C	ldc.r8	2
               12	0035	add
               13	0036	ldc.r8	0
               14	003F	ldloc	V_5 (5)
               15	0043	call	float64 [mscorlib]System.Math::Abs(float64)
               16	0048	sub
               17	0049	ldc.r8	1
               18	0052	add
               19	0053	dup
               20	0054	conv.r8
               21	0055	stloc	V_5 (5)
               22	0059	bge.s	25 (0062) ldc.i4 0x7EFBC126
               
             */
            OpCode[] pattern36 =
            {
                OpCodes.Ldloc,OpCodes.Call, OpCodes.Ldc_R4,OpCodes.Add,OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Ldc_R8,OpCodes.Ldloc,OpCodes.Call,OpCodes.Sub,OpCodes.Ldc_R8,OpCodes.Add,OpCodes.Dup,OpCodes.Conv_R8,OpCodes.Stloc,
            };
            OpCode[] ReplaceWit36h =
            {
                OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_0, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop, OpCodes.Nop,
                OpCodes.Nop
            };
            PatternsDictionary.Add("Pattern36", new Tuple<OpCode[], OpCode[]>(pattern36, ReplaceWit36h));
        }

        private static bool tester()
        {
            bool modified = false;
            int amount = 0;
            foreach (TypeDef typeDef in ModuleDefMD.GetTypes())
            {
                foreach (MethodDef methods in typeDef.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (int i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        if (methods.Body.Instructions[i].OpCode == OpCodes.Call &&
                            methods.Body.Instructions[i].Operand.ToString().Contains("Math::Abs"))
                        {
                            if (methods.Body.Instructions[i - 1].IsLdloc())
                            {
                                if (i > 1 && methods.Body.Instructions[i - 2].OpCode == OpCodes.Ldc_R8)
                                {
                                    methods.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 1].Operand = (double)10;
                                    amount++;
                                }

                                else if (IsConstant(methods.Body.Instructions[i + 1]) && IsArithmatic(methods.Body.Instructions[i + 2]))
                                {
                                    methods.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 1].Operand = (double)10;
                                    amount++;
                                }
                                else if (IsConstant(methods.Body.Instructions[i + 2]) && IsArithmatic(methods.Body.Instructions[i + 1]))
                                {
                                    methods.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 1].Operand = (double)10;
                                    amount++;
                                }
                            }
                            if (methods.Body.Instructions[i - 1].OpCode == OpCodes.Conv_R8 && methods.Body.Instructions[i - 2].IsLdloc())
                            {
                                if (i > 1 && methods.Body.Instructions[i - 3].OpCode == OpCodes.Ldc_R8)
                                {
                                    methods.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 2].Operand = (double)10;
                                    amount++;
                                }

                                else if (IsConstant(methods.Body.Instructions[i + 1]) && IsArithmatic(methods.Body.Instructions[i + 2]))
                                {
                                    methods.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 2].Operand = (double)10;
                                    amount++;
                                }
                                else if (IsConstant(methods.Body.Instructions[i + 2]) && IsArithmatic(methods.Body.Instructions[i + 1]))
                                {
                                    methods.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                                    methods.Body.Instructions[i - 2].Operand = (double)10;
                                    amount++;
                                }
                            }

                        }

                    }
                }
            }
            if (amount != 0)
                return true;
            else
                return false;
            //        Console.ReadLine();
        }

        public static bool IsArithmatic(Instruction ins)
        {
            return ins.OpCode == OpCodes.Add || ins.OpCode == OpCodes.Mul || ins.OpCode == OpCodes.Sub ||
                   ins.OpCode == OpCodes.Div;
        }
        public static bool IsConstant(Instruction ins)
        {
            if (ins.IsLdcI4() || ins.OpCode == OpCodes.Ldc_R4 || ins.OpCode == OpCodes.Ldc_R8)
                return true;
            return false;
        }
    }
}
