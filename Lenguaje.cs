//Mario Valdez Rico
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
//Requerimiento 1.- Actualizacion: 
//                  A) Agregar el residuo de la division en porfactor
//                  B) Agregar en instruccion los incrementos de termino y los incrementos
//                     de factor, a++,a--,a+=1,a-=1,a*=1,a/=1,a%=1
//                     en donde el 1 puede ser una expresion
//                  C)programar el destructor en la clase lexico
//                  para ejecutar el metodo cerrarArchivo
//                  #libreria especial? contenedor?
//Requerimiento 2.- Actualizacion la venganza:                 
//                  C) marcar errores semanticos cuando los incrementos de termino o incrementos de factor 
//                     superen el rango de la variable
//                  D) considerar el inciso b) y c) para el for
//                  E) que funcione el do y el while
//Requerimiento 3.- 
//                  A)Considerar las variables y los casteos de las exresiones matematicas en ensamblador
//                  B)Considerar el residuo de la division (residuo queda en DX y el resultado en AX)
//                  C)programar el printf y el scanf en assembler????????
//Requerimiento 4.- a)programar el else en assembler
//////////////////  b)programar el for en assembler
//Requerimiento 5.- a)programar el while en assembler
//                  b)programar el do while en assembler
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf;
        int cFor;
        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }
        //destructor
        /* ~Lenguaje()
         {
             Console.WriteLine("Destructor");
             cerrar();
         }*/

        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }
        private void variablesASM()
        {
            asm.WriteLine(";Variable");
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t" + v.getNombre() + " DW ?");
            }
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                    break;
                }
            }

        }
        private float getValor(string nombreVariable)
        {
            float valor = 0;
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreVariable)
                {
                    valor = v.getValor();
                    break;
                }

            }
            return valor;

        }
        private Variable.TipoDato getTipo(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return v.getTipo();

                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesASM();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("END");

        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                //if (getContenido() == "#")
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }

            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }
        //Bloque de instrucciones -> {ListaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }
        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        //Asignacion -> identificador = cadena | Expresion;
        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }
        private void Asignacion(bool evaluacion)
        {
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }
            string nombre = getContenido();
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                //log.WriteLine();
                //log.Write(getContenido() + " = ");
                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;
                if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
                {
                    //requerimiento 1.B
                    //requerimiento 1.C
                }
                Expresion();
                match(";");
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                //log.Write("= " + resultado);
                //log.WriteLine();
                //Console.WriteLine(dominante);
                //Console.WriteLine(evaluaNumero(resultado));
                if (dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }

                if (dominante <= getTipo(nombre))
                {
                    if (evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica: no podemos asignar un: <" + dominante + "> a un <" + getTipo(nombre) + "> en linea  " + linea, log);
                }

                asm.WriteLine("MOV " + nombre + ", AX");
            }
            else
            {
                Incremento(nombre, evaluacion);
                match(";");
            }
        }
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");

            bool validarWhile = Condicion("") && evaluacion;
            //requerimiento 4
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarWhile);
            }
            else
            {
                Instruccion(validarWhile);
            }
        }
        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            match("while");
            match("(");
            //requerimiento 4
            bool validarDo = Condicion("") && evaluacion;
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor++;
            asm.WriteLine(etiquetaInicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion);
            string nombre;
            int pos = posicion - 1;
            int lin = linea;
            bool validarFor = Condicion("");
            int cambio = 0;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                validarFor = Condicion("");
                match(";");
                nombre = getContenido();
                cambio = Incremento();
                //Requerimiento 1.D
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor && evaluacion);
                }
                else
                {
                    Instruccion(validarFor && evaluacion);
                }
                if (validarFor && evaluacion)
                {
                    modVariable(nombre, getValor(nombre) + cambio);
                }
                //Console.Write(getValor(nombre));
            } while (evaluacion && validarFor);
            asm.WriteLine(etiquetaFinFor + ":");
        }
        //Incremento -> Identificador ++ | --
        private int Incremento()
        {
            string variable = getContenido();
            int cambio = 0;
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }
            match(Tipos.Identificador);

            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++"); cambio = 1;
                }
                else
                {
                    match("--"); cambio = -1;
                }
            }
            else
            {
                match(Tipos.IncrementoTermino);
            }
            //Console.Write(getValor(variable));// esto es para debuguear la salida
            return cambio;
        }
        private void Incremento(string nombre, bool evaluacion)//este lleva asignacion
        {
            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) + 1);
                    }
                }
                if (getContenido()[0] == '-')
                {
                    match("--");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) - 1);
                    }
                }
                //no se si este bien 
                 if (getContenido()[0] == '+')
                {
                    match("+=");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) + 1);
                    }
                }
                if (getContenido()[0] == '-')
                {
                    match("-=");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) - 1);
                    }
                }
                if (getContenido()[0] == '*')
                {
                    match("*=");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) * 1);
                    }
                }
                if (getContenido()[0] == '/')
                {
                    match("/=");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) / 1);
                    }
                }
                if (getContenido()[0] == '%')
                {
                    match("%=");
                    if (evaluacion)
                    {
                        modVariable(nombre, getValor(nombre) % 1);
                    }
                }
                
            }
            else
            {
                match(Tipos.IncrementoTermino);
            }
        }
        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }
        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP AX");
            float e1 = stack.Pop();
            asm.WriteLine("POP BX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }

        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++cIf;
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion(etiquetaIf);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf && evaluacion);
            }
            else
            {
                Instruccion(validarIf && evaluacion);
            }
            if (getContenido() == "else")
            {
                //requerimiento 4
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf && evaluacion);
                }
                else
                {
                    Instruccion(!validarIf && evaluacion);
                }
            }
            asm.WriteLine(etiquetaIf + ":");
        }
        //Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {

                if (evaluacion)
                {
                    string cade = getContenido();
                    string CadenaLimpia = cade.TrimStart('"');
                    CadenaLimpia = CadenaLimpia.Remove(CadenaLimpia.Length - 1);
                    CadenaLimpia = CadenaLimpia.Replace("\\n", "\n");
                    CadenaLimpia = CadenaLimpia.Replace("\\t", "\t");
                    Console.Write(CadenaLimpia);
                }
                asm.WriteLine("PRINTN "+ getContenido());
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    Console.Write(resultado);
                    //codigo ensamblador para imprimir una variable
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf(bool evaluacion)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }

            if (evaluacion)
            {
                string val = "" + Console.ReadLine();
                //requerimiento 5
                if (Regex.IsMatch(val, @"^[0-9]+([.][0-9]+)?$"))//tiene que ser un numero con punto decimal
                {
                    modVariable(getContenido(), float.Parse(val));
                }
                else
                {
                    throw new Error("El valor que has introducido No es un numero <" + val + "> en la linea: " + linea, log);
                }
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV "+getContenido()+", CX");
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string Operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                //log.Write(Operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (Operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string Operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                //log.Write(Operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //requerimiento 1.A
                switch (Operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        stack.Push(n2 % n1);      //residuo?
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                //log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }

                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
                }
                //log.Write(getContenido() + " ");
                //Requerimiento 1 
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));//stack.Push(float.Parse(getContenido()));
                //requerimiento 3.a
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    //poner expresion
                    //Expresion();
                    //match(")");
                }
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    //esta seria mi funcion convert                    
                    dominante = casteo;
                    float cast = stack.Pop();
                    asm.WriteLine("POP AX");
                    switch (dominante)
                    {
                        case Variable.TipoDato.Char:
                            stack.Push((char)cast % 256);
                            break;
                        case Variable.TipoDato.Int:
                            stack.Push((int)cast % 65536);
                            break;
                        case Variable.TipoDato.Float:
                            stack.Push((float)cast);
                            break;
                    }
                    //requerimiento 2
                    //actualizar dominante
                    //saco un elemento del stack
                    //convierto ese valor al equivalente en casteo
                    //requerimiento 3;
                    //ejemplo si el casteo es (char) y el pop regresa un 256
                    //el valor equivalente en casteo es 0

                }
            }
        }
    }
}