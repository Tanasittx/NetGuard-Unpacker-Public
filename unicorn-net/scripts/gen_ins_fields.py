import sys

def main(args):
    if len(args) < 2:
        print("Must specify file defining instructions in command line argument.", file=sys.stderr)
        exit(1)

    ins_file = open(args[1], 'r')
    inses = ins_file.read().split(',')
    ins_file.close()

    insid = 1
    final_code = ""
    for ins in inses:
        ins = ins.strip()

        name = ins[ins.rindex('_') + 1:]
        code = "/// <summary>\n/// Represents the {0} instruction.\n/// </summary>\n".format(name)
        code += "public static readonly Instruction {0} = new Instruction({1});\n\n".format(name, insid)

        insid += 1
        final_code += code

    code_file = open(args[1] + '.cs', 'w')
    code_file.write(final_code);

if __name__ == "__main__":
    main(sys.argv)