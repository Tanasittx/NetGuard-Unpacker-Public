import sys
import os

from enum import Enum

class Register(object):
    def __init__(self, name, id):
        self.__name = name
        self.__id = id

    @property
    def name(self):
        return self.__name

    @property
    def id(self):
        return self.__id

    @id.setter
    def id(self, id):
        self.__id = id
    
    def __str__(self):
        return "reg({0}:{1})".format(self.name, self.id)

class TokenType(Enum):
    EOF = 0
    IDENT = 1
    INTEGER = 2
    EQUAL = 3
    COMMA = 4

class Token(object):
    def __init__(self, value, type):
        self.__value = value
        self.__type = type

    @property
    def value(self):
        return self.__value
        
    @property
    def type(self):
        return self.__type

    def __str__(self):
        return "tok({0}:'{1}')".format(self.__type, self.__value)

class Lexer(object):
    def __init__(self, src):
        self.__cur = None
        self.__src = src
        self.__pos = 0

        # Set the __cur character we're processing.
        self.__advance()

    def __skip_whitespace(self):
        while self.__cur.isspace():
            self.__advance()

    def __advance(self):
        if (self.__pos > len(self.__src) - 1):
            self.__cur = None
        else:
            self.__cur = self.__src[self.__pos]
            self.__pos += 1

    def next(self):
        # Figure out if we're lexing a comment.
        in_comment = True
        # Continueously skip comment lines and whitespaces.
        while in_comment:
            # EOF
            if self.__cur == None:
                return Token(None, TokenType.EOF)

            # Skip whitespace.
            self.__skip_whitespace()

            # Skip comments.
            # NOTE: Not checking for the second '/', because not really necessary.
            if self.__cur == '/':
                in_comment = True

                # Skip current line, assuming it started with '//'
                while self.__cur != '\n':
                    self.__advance()
            else:
                in_comment = False

        # Scan equal operator.
        if self.__cur == '=':
            self.__advance()
            return Token('=', TokenType.EQUAL)

        # Scan seperator.
        if self.__cur == ',':
            self.__advance()
            return Token(',', TokenType.COMMA)

        # Scan identifiers.
        if self.__cur.isalpha() or self.__cur == '_':
            ident = ''
            while self.__cur.isalpha() or self.__cur == '_' or self.__cur.isdigit():
                ident += self.__cur
                self.__advance()
            
            return Token(ident, TokenType.IDENT)

        # Scan integers.
        if self.__cur.isdigit():
            integer = ''
            while self.__cur.isdigit():
                integer += self.__cur
                self.__advance()

            return Token(integer, TokenType.INTEGER)

        raise Exception("Unexpected character: '{0}' index: {1}".format(str(self.__cur), str(self.__pos)))

class Parser(object):
    def __init__(self, lexer):
        self.__lexer = lexer
        self.__cur = self.__lexer.next()
    
    def __eat(self, type):
        if self.__cur.type != type:
            raise Exception("Unexpected token: {0}", str(self.__cur))
        self.__cur = self.__lexer.next()

    def parse(self):
        # Return None when we've reached EOF.
        if self.__cur.type == TokenType.EOF:
            return None

        ident = self.__cur.value
        self.__eat(TokenType.IDENT)

        if self.__cur.type == TokenType.COMMA:
            # => reg,
            self.__eat(TokenType.COMMA)
            return Register(ident, None)

        elif self.__cur.type == TokenType.EQUAL:
            self.__eat(TokenType.EQUAL)
            if self.__cur.type == TokenType.INTEGER:
                # => reg = INTEGER,
                integer = self.__cur.value
                self.__eat(TokenType.INTEGER)
                self.__eat(TokenType.COMMA)
                return Register(ident, int(integer))

            elif self.__cur.type == TokenType.IDENT:
                # => reg = reg2,
                ident2 = self.__cur.value
                self.__eat(TokenType.IDENT)
                self.__eat(TokenType.COMMA)
                return Register(ident, ident2)

        raise Exception()

def main(args):
    if len(args) < 2:
        print("error: no input file specified", file=sys.stderr)
        return 1

    src_filename = args[1]
    with open(src_filename, "r") as f:
        src = f.read()

    register_id = 1
    registers = {}

    lexer = Lexer(src)
    parser = Parser(lexer)

    # Parse registers and put them in a ditionary
    # mapping register names to register instances.
    register = parser.parse()
    while register != None:
        if register != None:
            registers[register.name] = register
        register = parser.parse()

    # Resolves the register IDs.
    for register_name, register in registers.items():
        if register.id == None:
            register.id = register_id
            register_id += 1
        elif type(register.id) is str:
            # NOTE: Does not support 'nested' references.
            register.id = registers[register.id].id
        elif type(register.id) is int:
            register_id = register.id + 1

    # Where the code generation begins.
    PREFIX = '_REG_'
    PREFIX_LEN = len(PREFIX)
    code = '// Generated by ' + os.path.basename(__file__) + '.\r\n'

    # Iterating over the registers again, cause why not.
    for register_name, register in registers.items():
        name = register_name[register_name.index(PREFIX) + PREFIX_LEN:]
        id = register.id
        if name[0].isdigit():
            name = '_' + name

        # Skip enum value used to indicating ending of register enum.
        if name == 'ENDING':
            continue

        code += "/// <summary>\n/// Gets or sets the value of {0} register.\n/// </summary>\n".format(name)
        code += "public long {0} \n{{\n".format(name)
        code += "    get {{ return Read({0}); }}\n".format(id)
        code += "    set {{ Write({0}, value); }}".format(id)
        code += "\n}\n\n"
    
    # Write to disk.
    out_filename = src_filename + ".cs"
    with open(out_filename, "w") as f:
        f.write(code)
    return 0
    
if __name__ == '__main__':
    main(sys.argv)
