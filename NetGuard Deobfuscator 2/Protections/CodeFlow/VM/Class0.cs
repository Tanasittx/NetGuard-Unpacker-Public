using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.VM
{
    internal static class Class_0
    {
        // Token: 0x0600023F RID: 575 RVA: 0x0000471D File Offset: 0x0000291D
        internal static int Method_0(string string_0, int int_0, Stream resouces)
        {
            return Class_0.Method_1(string_0, int_0, resouces);
        }

        // Token: 0x06000240 RID: 576 RVA: 0x00051A54 File Offset: 0x0004FC54
        internal static int Method_1(string string_0, int int_0, Stream resources)
        {
            List<double> list = new List<double>();
            foreach (string s in string_0.Split(new char[]
            {
            ','
            }))
            {
                list.Add(double.Parse(s, CultureInfo.InvariantCulture));
            }
            if (Class_0.Field_0 == null)
            {
                Class_0.Method_2(resources);
            }
            Class_0.Class_2 class_ = Class_0.Method_6(Class_0.Field_0, list, -1.0, 0.0);
            class_.Field_0.Add((double)int_0);
            return (int)Class_0.Method_7(class_).Field_6;
        }

        // Token: 0x06000241 RID: 577 RVA: 0x00051AE0 File Offset: 0x0004FCE0
        internal static void Method_2(Stream resources)
        {
            List<double> list = new List<double>();
            foreach (string s in new StreamReader(resources).ReadToEnd().Split(new char[]
            {
            ','
            }))
            {
                list.Add(double.Parse(s, CultureInfo.InvariantCulture));
            }
            Class_0.Field_0 = new Class_0.Class_1(list);
        }

        // Token: 0x06000242 RID: 578 RVA: 0x00004726 File Offset: 0x00002926
        internal static double Method_3(Class_0.Class_2 class_2_0)
        {
            double result = class_2_0.Field_2[class_2_0.Field_2.Count - 1];
            class_2_0.Field_2.Remove(class_2_0.Field_2[class_2_0.Field_2.Count - 1]);
            return result;
        }

        // Token: 0x06000243 RID: 579 RVA: 0x00004764 File Offset: 0x00002964
        internal static void Method_4(Class_0.Class_2 class_2_0, double double_0)
        {
            class_2_0.Field_2.Add(double_0);
        }

        // Token: 0x06000244 RID: 580 RVA: 0x00051B4C File Offset: 0x0004FD4C
        internal static double Method_5(Class_0.Class_2 class_2_0)
        {
            List<double> field_ = class_2_0.Field_1;
            double num = class_2_0.Field_3 + 1.0;
            class_2_0.Field_3 = num;
            return field_[(int)num];
        }

        // Token: 0x06000245 RID: 581 RVA: 0x00051B7C File Offset: 0x0004FD7C
        internal static Class_0.Class_2 Method_6(Class_0.Class_1 class_1_0, List<double> list_0, double double_0, double double_1)
        {
            return new Class_0.Class_2
            {
                Field_7 = class_1_0,
                Field_1 = list_0,
                Field_3 = double_0,
                Field_5 = 0.0,
                Field_4 = -1.0,
                Field_0 = new List<double>((int)double_1),
                Field_2 = new List<double>(Class_0.Field_1)
            };
        }

        // Token: 0x06000246 RID: 582 RVA: 0x00051BEC File Offset: 0x0004FDEC
        internal static Class_0.Class_2 Method_7(Class_0.Class_2 class_2_0)
        {
            bool flag = false;
            while (!flag)
            {
                List<double> field_ = class_2_0.Field_1;
                double num = class_2_0.Field_3 + 1.0;
                class_2_0.Field_3 = num;
                double num2 = field_[(int)num];
                if (num2 == class_2_0.Field_7.Field_0)
                {
                    double num3 = Class_0.Method_3(class_2_0);
                    double num4 = Class_0.Method_3(class_2_0);
                    Class_0.Method_4(class_2_0, num4 + num3);
                }
                else if (num2 != class_2_0.Field_7.Field_23)
                {
                    if (num2 == class_2_0.Field_7.Field_1)
                    {
                        double num3 = Class_0.Method_3(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, num4 - num3);
                    }
                    else if (num2 == class_2_0.Field_7.Field_2)
                    {
                        double num3 = Class_0.Method_3(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, num4 + num3);
                    }
                    else if (num2 == class_2_0.Field_7.Field_3)
                    {
                        double num3 = Class_0.Method_3(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, num4 / num3);
                    }
                    else if (num2 == class_2_0.Field_7.Field_4)
                    {
                        double num3 = Class_0.Method_5(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, (double)((num4 < num3) ? 1 : 0));
                        class_2_0.Field_3 += 1.0;
                    }
                    else if (num2 == class_2_0.Field_7.Field_5)
                    {
                        double num3 = Class_0.Method_5(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, (double)((num4 > num3) ? 1 : 0));
                        class_2_0.Field_3 += 1.0;
                    }
                    else if (num2 == class_2_0.Field_7.Field_6)
                    {
                        double num3 = Class_0.Method_5(class_2_0);
                        if (num3 == class_2_0.Field_7.Field_17)
                        {
                            num3 = class_2_0.Field_0[0];
                        }
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, (double)(num4.Equals(num3) ? 1 : 0));
                        class_2_0.Field_3 += 1.0;
                    }
                    else if (num2 == class_2_0.Field_7.Field_7)
                    {
                        class_2_0.Field_3 = Class_0.Method_5(class_2_0);
                    }
                    else if (num2 == class_2_0.Field_7.Field_8)
                    {
                        double field_2 = Class_0.Method_5(class_2_0);
                        if (Class_0.Method_3(class_2_0) == 1.0)
                        {
                            class_2_0.Field_3 = field_2;
                        }
                        class_2_0.Field_3 += 1.0;
                    }
                    else if (num2 == class_2_0.Field_7.Field_9)
                    {
                        double field_2 = Class_0.Method_5(class_2_0);
                        if (Class_0.Method_3(class_2_0) == 0.0)
                        {
                            class_2_0.Field_3 = field_2;
                        }
                        else
                        {
                            class_2_0.Field_3 += 1.0;
                        }
                    }
                    else if (num2 == class_2_0.Field_7.Field_10)
                    {
                        double num5 = Class_0.Method_5(class_2_0);
                        if (num5 == class_2_0.Field_7.Field_17)
                        {
                            num5 = class_2_0.Field_0[0];
                        }
                        Class_0.Method_4(class_2_0, num5);
                        class_2_0.Field_3 += 1.0;
                    }
                    else if (num2 == class_2_0.Field_7.Field_11)
                    {
                        class_2_0.Field_2.Remove(class_2_0.Field_2[class_2_0.Field_2.Count - 1]);
                    }
                    else if (num2 == class_2_0.Field_7.Field_12)
                    {
                        class_2_0.Field_6 = Class_0.Method_3(class_2_0);
                        flag = true;
                    }
                    else if (num2 == class_2_0.Field_7.Field_13)
                    {
                        double num5 = class_2_0.Field_2[class_2_0.Field_2.Count - 1];
                        class_2_0.Field_0.Insert(0, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_14)
                    {
                        double num5 = Class_0.Method_5(class_2_0);
                        class_2_0.Field_0.Insert(1, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_15)
                    {
                        double num5 = Class_0.Method_5(class_2_0);
                        class_2_0.Field_0.Insert(2, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_16)
                    {
                        double num5 = Class_0.Method_5(class_2_0);
                        class_2_0.Field_0.Insert(3, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_17)
                    {
                        double num5 = class_2_0.Field_0[0];
                        Class_0.Method_4(class_2_0, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_18)
                    {
                        double num5 = class_2_0.Field_0[1];
                        Class_0.Method_4(class_2_0, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_19)
                    {
                        double num5 = class_2_0.Field_0[2];
                        Class_0.Method_4(class_2_0, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_20)
                    {
                        double num5 = class_2_0.Field_0[3];
                        Class_0.Method_4(class_2_0, num5);
                    }
                    else if (num2 == class_2_0.Field_7.Field_21)
                    {
                        double num3 = Class_0.Method_3(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, (double)((int)num4 << (int)num3));
                    }
                    else if (num2 == class_2_0.Field_7.Field_22)
                    {
                        double num3 = Class_0.Method_3(class_2_0);
                        double num4 = Class_0.Method_3(class_2_0);
                        Class_0.Method_4(class_2_0, (double)((int)num4 >> (int)num3));
                    }
                }
            }
            return class_2_0;
        }

        // Token: 0x06000247 RID: 583 RVA: 0x00004772 File Offset: 0x00002972
        // Note: this type is marked as 'beforefieldinit'.
        static Class_0()
        {
        }

        // Token: 0x0400053F RID: 1343
        internal static Class_0.Class_1 Field_0;

        // Token: 0x04000540 RID: 1344
        internal static int Field_1 = 100;

        // Token: 0x02000123 RID: 291
        internal class Class_1
        {
            // Token: 0x06000248 RID: 584 RVA: 0x000521F4 File Offset: 0x000503F4
            internal Class_1(List<double> list_0)
            {
                this.Field_0 = list_0[1];
                this.Field_1 = list_0[2];
                this.Field_2 = list_0[3];
                this.Field_3 = list_0[4];
                this.Field_4 = list_0[5];
                this.Field_5 = list_0[6];
                this.Field_6 = list_0[7];
                this.Field_7 = list_0[8];
                this.Field_8 = list_0[9];
                this.Field_9 = list_0[10];
                this.Field_10 = list_0[11];
                this.Field_11 = list_0[12];
                this.Field_12 = list_0[13];
                this.Field_13 = list_0[14];
                this.Field_14 = list_0[15];
                this.Field_15 = list_0[16];
                this.Field_16 = list_0[17];
                this.Field_17 = list_0[18];
                this.Field_18 = list_0[19];
                this.Field_19 = list_0[20];
                this.Field_20 = list_0[21];
                this.Field_21 = list_0[22];
                this.Field_22 = list_0[23];
                this.Field_23 = list_0[24];
            }

            // Token: 0x04000541 RID: 1345
            internal double Field_0 = 1.0;

            // Token: 0x04000542 RID: 1346
            internal double Field_1 = 2.0;

            // Token: 0x04000543 RID: 1347
            internal double Field_2 = 3.0;

            // Token: 0x04000544 RID: 1348
            internal double Field_3 = 4.0;

            // Token: 0x04000545 RID: 1349
            internal double Field_4 = 5.0;

            // Token: 0x04000546 RID: 1350
            internal double Field_5 = 6.0;

            // Token: 0x04000547 RID: 1351
            internal double Field_6 = 7.0;

            // Token: 0x04000548 RID: 1352
            internal double Field_7 = 8.0;

            // Token: 0x04000549 RID: 1353
            internal double Field_8 = 9.0;

            // Token: 0x0400054A RID: 1354
            internal double Field_9 = 10.0;

            // Token: 0x0400054B RID: 1355
            internal double Field_10 = 11.0;

            // Token: 0x0400054C RID: 1356
            internal double Field_11 = 12.0;

            // Token: 0x0400054D RID: 1357
            internal double Field_12 = 13.0;

            // Token: 0x0400054E RID: 1358
            internal double Field_13 = 14.0;

            // Token: 0x0400054F RID: 1359
            internal double Field_14 = 15.0;

            // Token: 0x04000550 RID: 1360
            internal double Field_15 = 16.0;

            // Token: 0x04000551 RID: 1361
            internal double Field_16 = 17.0;

            // Token: 0x04000552 RID: 1362
            internal double Field_17 = 18.0;

            // Token: 0x04000553 RID: 1363
            internal double Field_18 = 19.0;

            // Token: 0x04000554 RID: 1364
            internal double Field_19 = 20.0;

            // Token: 0x04000555 RID: 1365
            internal double Field_20 = 21.0;

            // Token: 0x04000556 RID: 1366
            internal double Field_21 = 22.0;

            // Token: 0x04000557 RID: 1367
            internal double Field_22 = 23.0;

            // Token: 0x04000558 RID: 1368
            internal double Field_23 = 24.0;
        }

        // Token: 0x02000124 RID: 292
        internal struct Class_2
        {
            // Token: 0x04000559 RID: 1369
            internal List<double> Field_0;

            // Token: 0x0400055A RID: 1370
            internal List<double> Field_1;

            // Token: 0x0400055B RID: 1371
            internal List<double> Field_2;

            // Token: 0x0400055C RID: 1372
            internal double Field_3;

            // Token: 0x0400055D RID: 1373
            internal double Field_4;

            // Token: 0x0400055E RID: 1374
            internal double Field_5;

            // Token: 0x0400055F RID: 1375
            internal double Field_6;

            // Token: 0x04000560 RID: 1376
            internal Class_0.Class_1 Field_7;
        }
    }
}
