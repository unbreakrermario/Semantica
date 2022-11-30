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
        int cDo;
        int cWhile;
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
            BloqueInstrucciones(true, true);
        }
        //Bloque de instrucciones -> {ListaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool executedPrintAsm)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, executedPrintAsm);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool executedPrintAsm)
        {
            Instruccion(evaluacion, executedPrintAsm);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, executedPrintAsm);
            }
        }
        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool executedPrintAsm)
        {
            Instruccion(evaluacion, executedPrintAsm);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, executedPrintAsm);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool executedPrintAsm)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, executedPrintAsm);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, executedPrintAsm);
            }
            else
            {
                Asignacion(evaluacion, executedPrintAsm);
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
        private void Asignacion(bool evaluacion, bool executedPrintAsm)
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
                Incremento(nombre, evaluacion, executedPrintAsm);
                match(";");
            }
            else
            {
                // Console.WriteLine("entro directo");
                //log.WriteLine();
                //log.Write(getContenido() + " = ");
                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;

                Expresion(executedPrintAsm);
                match(";");
                float resultado = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP AX");
                }
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }

        }
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool executedPrintAsm)
        {
            string etiquetaInicioWhile = "inicioWhile" + cWhile;
            string etiquetaFinWhile = "finWhile" + cWhile;
            if(executedPrintAsm)
            {
                cWhile++;
            }
            if (executedPrintAsm)
            {
                asm.WriteLine(etiquetaInicioWhile + ":");
            }
            match("while");
            match("(");
            int pos = posicion - 1;
            int lin = linea;
            bool validarWhile;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                //Console.WriteLine(getContenido());//debug
                validarWhile = Condicion(etiquetaFinWhile, executedPrintAsm);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarWhile && evaluacion, executedPrintAsm);
                }
                else
                {
                    Instruccion(validarWhile && evaluacion, executedPrintAsm);
                }
                if (executedPrintAsm)
                {
                    asm.WriteLine("JMP " + etiquetaInicioWhile);
                    asm.WriteLine(etiquetaFinWhile + ":");
                    executedPrintAsm = false;//aqui es donde se cambia el estado de la bandera para que se imprima solo una vez
                    //lo de ensamblador
                }
            } while (evaluacion && validarWhile);
        }
        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool executedPrintAsm)
        {

            string etiquetaInicioDo = "inicioDo" + cDo;
            string etiquetaFinDo = "finDo" + cDo;
            if (executedPrintAsm)
            {
                cDo++;
            }
            match("do");
            int pos = posicion - 1;
            int lin = linea;
            bool validarDo;
            if (executedPrintAsm)
            {
                asm.WriteLine(etiquetaInicioDo + ":");
            }
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, executedPrintAsm);
                }
                else
                {
                    Instruccion(evaluacion, executedPrintAsm);
                }
                match("while");
                match("(");
                validarDo = Condicion(etiquetaFinDo, executedPrintAsm);// && evaluacion
                match(")");
                match(";");
                if (executedPrintAsm)
                {
                    asm.WriteLine("JMP " + etiquetaInicioDo);
                    asm.WriteLine(etiquetaFinDo + ":");
                    executedPrintAsm = false;//aqui es donde se cambia el estado de la bandera para que se imprima solo una vez
                    //lo de ensamblador
                }
            } while (evaluacion && validarDo);

        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool executedPrintAsm)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor;
            if (executedPrintAsm)
            {
                cFor++;
            }
            match("for");
            match("(");
            Asignacion(evaluacion, executedPrintAsm);
            if (executedPrintAsm)
            {
                asm.WriteLine(etiquetaInicioFor + ":");
            }
            string nombreContenido;
            string operador;
            int pos = posicion - 1;
            int lin = linea;
            bool validarFor;
            float[] valores = new float[2];
            float cambio = 0;
            float resultado = 0;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
                posicion = pos;
                linea = lin;
                NextToken();
                validarFor = Condicion(etiquetaFinFor, executedPrintAsm);
                match(";");
                nombreContenido = getContenido();
                match(Tipos.Identificador);
                operador = getContenido();
                if (getClasificacion() == Tipos.IncrementoTermino)
                {
                    match(Tipos.IncrementoTermino);

                }
                else if (getClasificacion() == Tipos.IncrementoFactor)
                {
                    match(Tipos.IncrementoFactor);

                }
                else
                {
                    throw new Error("error de sintaxis, se espera IncrementoTermino o IncrementoFactor en la linea: " + linea, log);
                }
                valores = (float[])Incremento(nombreContenido, operador, false);
                cambio = valores[0];
                resultado = valores[1];
                //Requerimiento 1.D
                //Console.WriteLine(getContenido());
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor && evaluacion, executedPrintAsm);
                }
                else
                {
                    Instruccion(validarFor && evaluacion, executedPrintAsm);
                }
                if (validarFor && evaluacion)
                {
                    Console.WriteLine( getValor(nombreContenido));
                    if(operador == "++"|| operador == "--")  
                        modVariable(nombreContenido, cambio);
                    else{
                        //pendiente arreglar lo del mod
                    }
                    //Console.WriteLine(cambio);
                }
                if (executedPrintAsm)
                {
                    //aqui debe ir el incremento segun el tipo de operador
                    switch (operador)
                    {
                        case "++":
                            asm.WriteLine("INC " + nombreContenido);
                            break;
                        case "--":
                            asm.WriteLine("DEC " + nombreContenido);
                            break;
                        case "+=":
                            asm.WriteLine("MOV AX, " + nombreContenido);
                            asm.WriteLine("ADD AX, " + resultado);
                            asm.WriteLine("MOV " + nombreContenido + ", AX");
                            break;
                        case "-=":
                            asm.WriteLine("MOV AX, " + nombreContenido);
                            asm.WriteLine("SUB AX, " + resultado);
                            asm.WriteLine("MOV " + nombreContenido + ", AX");
                            break;
                        case "*=":
                            asm.WriteLine("MOV AX, " + nombreContenido);
                            asm.WriteLine("MOV BX, " + resultado);
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("MOV " + nombreContenido + ", AX");
                            break;
                        case "/=":
                            asm.WriteLine("MOV AX, " + nombreContenido);
                            asm.WriteLine("MOV BX, " + resultado);
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("MOV " + nombreContenido + ", AX");
                            break;
                        case "%=":
                            asm.WriteLine("MOV AX, " + nombreContenido);
                            asm.WriteLine("MOV BX, " + resultado);
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("MOV " + nombreContenido + ", DX");
                            break;
                    }
                    asm.WriteLine("JMP " + etiquetaInicioFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                    executedPrintAsm = false;//aqui es donde se cambia el estado de la bandera para que se imprima solo una vez
                    //lo de ensamblador
                }
            } while (evaluacion && validarFor);

        }

        //Incremento -> Identificador ++ | --
        private Array Incremento(string variable, string operador, bool executedPrintAsm)
        {
            float[] arreglo = new float[2];
            arreglo[0] = 0;
            float cambio = 0;
            if (!existeVariable(variable))
            {
                throw new Error("No existe la variable <" + variable + ">en la linea: " + linea, log);
            }
            if (operador == "++")
            {
                if (evaluaSemantica(variable, getValor(variable) + 1))
                {
                    cambio = getValor(variable) + 1;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }
            }
            else if (operador == "--")
            {
                if (evaluaSemantica(variable, getValor(variable) - 1))
                {
                    cambio = getValor(variable) - 1;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }

            }
            else if (operador == "+=")
            {

                Expresion(false);
                float resultado = stack.Pop();
                arreglo[1] = resultado;
                if (evaluaSemantica(variable, getValor(variable) + resultado))
                {
                    cambio = getValor(variable) + resultado;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }

            }
            else if (operador == "-=")
            {

                Expresion(false);
                float resultado = stack.Pop();
                arreglo[1] = resultado;
                if (evaluaSemantica(variable, getValor(variable) - resultado))
                {

                    cambio = getValor(variable) - resultado;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }

            }
            else if (operador == "*=")
            {

                Expresion(false);
                float resultado = stack.Pop();
                arreglo[1] = resultado;
                if (evaluaSemantica(variable, getValor(variable) * resultado))
                {
                    cambio = getValor(variable) * resultado;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }

            }
            else if (operador == "/=")
            {

                Expresion(false);
                float resultado = stack.Pop();
                arreglo[1] = resultado;
                if (evaluaSemantica(variable, getValor(variable) / resultado))
                {
                    cambio = getValor(variable) / resultado;
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }

            }
            else if (operador == "%=")
            {

                Expresion(false);
                float resultado = stack.Pop();
                arreglo[1] = resultado;
                if (evaluaSemantica(variable, getValor(variable) % resultado))
                {
                    cambio = getValor(variable) % resultado;
                    
                }
                else
                {
                    throw new Error("Error de semantica: el incremento supera el rango de la variable <" + variable + "> en la linea: " + linea, log);
                }
            }
            //Console.Write(getValor(variable));// esto es para debuguear la salida
            arreglo[0] = cambio;
            return arreglo;
        }
        private void Incremento(string nombre, bool evaluacion, bool executedPrintAsm)//este lleva asignacion
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("INC " + nombre);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("DEC " + nombre);
                }
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion(false);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("ADD AX, " + valor);
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion(false);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("SUB AX, " + valor);
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion(false);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("MOV BX, " + valor);
                    asm.WriteLine("MUL BX");
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion(false);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("MOV BX, " + valor);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion(false);
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
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("MOV BX, " + valor);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("MOV " + nombre + ", DX");//aqui la diferencia del div es que en division usa AX
                    //lo que es la parte

                }
            }
        }
        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool executedPrintAsm)
        {
            match("switch");
            match("(");
            Expresion(executedPrintAsm);
            stack.Pop();
            if (executedPrintAsm)
            {
                asm.WriteLine("POP AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, executedPrintAsm);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, executedPrintAsm);
                }
                else
                {
                    Instruccion(evaluacion, executedPrintAsm);
                }
            }
            match("}");
        }
        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool executedPrintAsm)
        {
            match("case");
            Expresion(executedPrintAsm);
            stack.Pop();
            if (executedPrintAsm)
            {
                asm.WriteLine("POP AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, executedPrintAsm);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, executedPrintAsm);
            }
        }

        //(Condicion) -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool executedPrintAsm)
        {
            Expresion(executedPrintAsm);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(executedPrintAsm);
            float e2 = stack.Pop();
            if (executedPrintAsm)
            {
                asm.WriteLine("POP BX");
            }
            float e1 = stack.Pop();
            if (executedPrintAsm)
            {
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    return e1 == e2;
                case ">":
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                case ">=":
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<":
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case "<=":
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                default:
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }

        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool executedPrintAsm)
        {
            string etiquetaIf = "if" + ++cIf;
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion(etiquetaIf, executedPrintAsm);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf && evaluacion, executedPrintAsm);
            }
            else
            {
                Instruccion(validarIf && evaluacion, executedPrintAsm);
            }
            if (getContenido() == "else")
            {
                //requerimiento 4
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf && evaluacion, executedPrintAsm);
                }
                else
                {
                    Instruccion(!validarIf && evaluacion, executedPrintAsm);
                }
            }
            if (executedPrintAsm)
            {
                asm.WriteLine(etiquetaIf + ":");
            }
        }
        //Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion, bool executedPrintAsm)
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
                if (executedPrintAsm)
                {
                    string cadena = "";
                    // asm.WriteLine("PRINT " + getContenido());
                    for (int i = 0; i < CadenaLimpia.Length; i++)
                    {
                        if (CadenaLimpia[i] != '\n')
                        {
                            cadena += CadenaLimpia[i];
                        }
                        else if (CadenaLimpia[i] == '\n')
                        {
                            if (cadena != "")
                            {
                                asm.WriteLine("PRINT " + "'" + cadena + "'");
                            }
                            asm.WriteLine("PRINTN ");
                            cadena = "";
                        }
                    }
                    if (cadena != "")
                    {
                        asm.WriteLine("PRINT " + "'" + cadena + "'");
                    }
                }
                //asm.WriteLine("PRINTN " + getContenido());
                /*for (int i = 0; i < getContenido().Length; i++)
                {
                    if (getContenido()[i] != '\n')
                    {
                        asm.WriteLine("PRINT " + getContenido());
                    }
                    else if (getContenido()[i] == '\n')
                    {
                        asm.WriteLine("PRINTN " + getContenido());
                    }

                }*/
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(executedPrintAsm);
                float resultado = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP AX");
                }
                if (evaluacion)
                {
                    Console.Write(resultado);
                }
                if (executedPrintAsm)
                {
                    asm.WriteLine("CALL PRINT_NUM");
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf(bool evaluacion, bool executedPrintAsm)
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
            }
            if (executedPrintAsm)
            {
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }
        //Expresion -> Termino MasTermino
        private void Expresion(bool executedPrintAsm)
        {
            Termino(executedPrintAsm);
            MasTermino(executedPrintAsm);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool executedPrintAsm)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string Operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(executedPrintAsm);
                //log.Write(Operador + " ");
                float n1 = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP AX");
                }
                switch (Operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if (executedPrintAsm)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if (executedPrintAsm)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool executedPrintAsm)
        {
            Factor(executedPrintAsm);
            PorFactor(executedPrintAsm);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool executedPrintAsm)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string Operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(executedPrintAsm);
                //log.Write(Operador + " ");
                float n1 = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (executedPrintAsm)
                {
                    asm.WriteLine("POP AX");
                }
                //requerimiento 1.A
                switch (Operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if (executedPrintAsm)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if (executedPrintAsm)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if (executedPrintAsm)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool executedPrintAsm)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                //log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }

                stack.Push(float.Parse(getContenido()));
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
                }
                //log.Write(getContenido() + " ");
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));//stack.Push(float.Parse(getContenido()));
                //requerimiento 3.a
                if (executedPrintAsm)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
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
                }
                Expresion(executedPrintAsm);
                match(")");
                if (huboCasteo)
                {
                    //esta seria mi funcion convert                    
                    dominante = casteo;
                    float cast = stack.Pop();
                    if (executedPrintAsm)
                    {
                        asm.WriteLine("POP AX");
                    }
                    switch (dominante)
                    {
                        case Variable.TipoDato.Char:
                            stack.Push((char)cast % 256);
                            if (executedPrintAsm)
                            {
                                asm.WriteLine("MOV AL, 0");
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                        case Variable.TipoDato.Int:
                            stack.Push((int)cast % 65536);
                            if (executedPrintAsm)
                            {
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                        case Variable.TipoDato.Float:
                            stack.Push((float)cast);
                            if (executedPrintAsm)
                            {
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                    }
                }
            }
        }
    }
}