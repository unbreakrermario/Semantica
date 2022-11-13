//Mario Valdez Rico
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
//Requerimiento 1.- Actualizacion: **
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
//                  A)Considerar las variables y los casteos de las expresiones matematicas en ensamblador
//                  B)Considerar el residuo de la division (residuo queda en DX y el resultado en AX)
//                  C)programar el printf y el scanf en assembler????????
//Requerimiento 4.- 
//                  a)programar el else en assembler
//                  b)programar el for en assembler
//para los ciclos hacer un chequeo del incremento que este sea == 0 y un bool para que sea solo una vez
//Requerimiento 5.- 
//                  a)programar el while en assembler
//                  b)programar el do while en assembler
//donde haya un asm.writeline poner una bandera para que fucione
//name bandera executedPrintAsm
namespace Semantica
{
    public class Lenguaje : Sintaxis//,IDisposable
    {
        /*public void Dispose()
        {
            Console.WriteLine("Se ha liberado la memoria");
            cerrar();
            // Dispose of unmanaged resources.
            // Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this); 
        }*/
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
        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }

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
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
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
            bool bandera = false;
            Variable.TipoDato tipoDato = getTipo(variable);
            if (evaluaNumero(resultado) <= getTipo(variable))
            {
                bandera = true;
            }
            return bandera;
            //con este checamos lo del error semantico
            //requerimiento 2.C
        }
        private void Asignacion(bool evaluacion)
        {
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }
            string nombre = getContenido();
            match(Tipos.Identificador);
            //Console.WriteLine("no entro");
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                //requerimiento 1.b
                //Console.WriteLine("entro?");
                Incremento(nombre, evaluacion);
                match(";");
            }
            else
            {
                // Console.WriteLine("entro directo");
                //log.WriteLine();
                //log.Write(getContenido() + " = ");
                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;

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

        }
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            int pos = posicion - 1;
            int lin = linea;
            bool validarWhile = Condicion("");
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                //Console.WriteLine(getContenido());//debug
                validarWhile = Condicion("");
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarWhile && evaluacion);
                }
                else
                {
                    Instruccion(validarWhile && evaluacion);
                }
            } while (evaluacion && validarWhile);
        }
        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            int pos = posicion - 1;
            int lin = linea;
            bool validarDo;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
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
                validarDo = Condicion("") && evaluacion;
                match(")");
                match(";");
            } while (evaluacion && validarDo);
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
            string nombreContenido;
            int pos = posicion - 1;
            int lin = linea;
            bool validarFor = Condicion("");
            float cambio = 0;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                validarFor = Condicion("");
                match(";");
                nombreContenido = getContenido();
                cambio = Incremento();
                //Requerimiento 1.D
                //Console.WriteLine(getContenido());
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
                    modVariable(nombreContenido, +cambio);
                }
                //Console.Write(getValor(nombre));
            } while (evaluacion && validarFor);
            asm.WriteLine(etiquetaFinFor + ":");
        }
        //Incremento -> Identificador ++ | --
        private float Incremento()
        {
            string variable = getContenido();
            float cambio = 0;
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }
            match(Tipos.Identificador);

            if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    if (evaluaSemantica(variable, getValor(variable) + 1))
                    {
                        //aqui falta el stack.pop
                        match("++"); cambio = getValor(variable) + 1;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }
                }
                if (getContenido() == "--")
                {
                    if (evaluaSemantica(variable, getValor(variable) - 1))
                    {
                        match("--"); cambio = getValor(variable) - 1;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }

                }
                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion();
                    float resultado = stack.Pop();
                    if (evaluaSemantica(variable, getValor(variable) + resultado))
                    {

                        cambio = getValor(variable) + resultado;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }

                }
                if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    float resultado = stack.Pop();
                    if (evaluaSemantica(variable, getValor(variable) - resultado))
                    {

                        cambio = getValor(variable) - resultado;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }

                }
                if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    float resultado = stack.Pop();
                    if (evaluaSemantica(variable, getValor(variable) * resultado))
                    {
                        cambio = getValor(variable) * resultado;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }

                }
                if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    float resultado = stack.Pop();
                    if (evaluaSemantica(variable, getValor(variable) / resultado))
                    {
                        cambio = getValor(variable) / resultado;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }

                }
                if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    float resultado = stack.Pop();
                    if (evaluaSemantica(variable, getValor(variable) % resultado))
                    {
                        cambio = getValor(variable) % resultado;
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                    }
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
            //if con evalua semantica para evaluar si el incremento supera el rango de la variable

            if (getContenido() == "++")
            {
                //Console.WriteLine(getContenido());
                match("++");
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) + 1))
                    {
                        modVariable(nombre, getValor(nombre) + 1);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    // modVariable(nombre, getValor(nombre) + 1);
                }
            }
            else if (getContenido() == "--")
            {
                match("--");
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) - 1))
                    {
                        modVariable(nombre, getValor(nombre) - 1);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    //modVariable(nombre, getValor(nombre) - 1);

                }
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                float valor = stack.Pop();

                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) + valor))
                    {
                        modVariable(nombre, getValor(nombre) + valor);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    //modVariable(nombre, getValor(nombre) + valor);
                }
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                float valor = stack.Pop();
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) - valor))
                    {
                        modVariable(nombre, getValor(nombre) - valor);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    //modVariable(nombre, getValor(nombre) - valor);
                }
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
                float valor = stack.Pop();
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) * valor))
                    {
                        modVariable(nombre, getValor(nombre) * valor);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    // modVariable(nombre, getValor(nombre) * valor);
                }
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
                float valor = stack.Pop();
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) / valor))
                    {
                        modVariable(nombre, getValor(nombre) / valor);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    // modVariable(nombre, getValor(nombre) / valor);
                }
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();
                float valor = stack.Pop();
                if (evaluacion)
                {
                    if (evaluaSemantica(nombre, getValor(nombre) % valor))
                    {
                        modVariable(nombre, getValor(nombre) % valor);
                    }
                    else
                    {
                        throw new Error("Error de semantica: el incremento supera el rango de la variable <" + nombre + "> en la linea: " + linea, log);
                    }
                    // modVariable(nombre, getValor(nombre) % valor);
                }
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
                string cade = getContenido();
                string CadenaLimpia = cade.TrimStart('"');
                CadenaLimpia = CadenaLimpia.Remove(CadenaLimpia.Length - 1);
                CadenaLimpia = CadenaLimpia.Replace("\\n", "\n");
                CadenaLimpia = CadenaLimpia.Replace("\\t", "\t");
                if (evaluacion)//visual
                {
                    Console.Write(CadenaLimpia);
                }
                //PENDIENTE NO QUITA 
                //asm.WriteLine("PRINT " + getContenido());
                //asm.WriteLine("PRINTN " + getContenido());
                for (int i = 0; i < getContenido().Length; i++)
                {
                    if (getContenido()[i] != '\n')
                    {
                        asm.WriteLine("PRINT " + getContenido());
                    }
                    else if (getContenido()[i] != '\n')
                    {
                        asm.WriteLine("PRINTN " + getContenido());
                    }

                }
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
                asm.WriteLine("PRINT_NUM");
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
                asm.WriteLine("MOV " + getContenido() + ", CX");
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
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");//residuo (dudas)
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
                            asm.WriteLine("MOV AL, 0");
                            asm.WriteLine("PUSH AX");
                            break;
                        case Variable.TipoDato.Int:
                            stack.Push((int)cast % 65536);
                            asm.WriteLine("PUSH AX");
                            break;
                        case Variable.TipoDato.Float:
                            stack.Push((float)cast);
                            asm.WriteLine("PUSH AX");
                            break;
                    }
                }
            }
        }
    }
}