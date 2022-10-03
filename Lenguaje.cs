//Mario Valdez Rico
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
//Requerimiento 1.- actualizar el dominante para variables en la expresion***
//                  Ejemplo: float x ; char y ; y=x ;
//Requerimiento 2.- actualizar el dominante para el casteo y el valor de la subexpresion*
//Requerimiento 3.- programar un metodo de conversion de un valor a un tipo de dato
//                  private float convert(float valor,string tipoDato) 
//                  deberan usar el residuo de la division %255, %65535
//Requerimiento 4.- evaluar nuevamente la condicion del if-else, while, for, do while, con
//                  con respecto al parametro que recibe
//Requerimiento 5.- levantar una excepcion en el scanf cuando la captura no sea numero***
//Requerimiento 6.- ejecutar el For();
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

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
                    //return valor;
                }

            }
            return valor;
            //return 0;
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
            Libreria();
            Variables();
            Main();
            displayVariables();
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
            log.WriteLine();
            log.Write(getContenido() + " = ");
            string nombre = getContenido();
            match(Tipos.Identificador);
            match(Tipos.Asignacion);
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.Write("= " + resultado);
            log.WriteLine();
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

        }
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");

            bool validarWhile = Condicion();
            //requerimiento 4
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
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
            bool validarDo = Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            //requerimiento 4
            //requerimiento 6: 
            //a) nescesito guardar la poscicion de lectura en el archivo
            bool validarFor = Condicion();
            //b) metemos un ciclo while despues de validar for
            //while()
            //{
            match(";");
            Incremento(evaluacion);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            //c) Regresar a la posicion de lectura del archivo
            //d) sacar otro token
            //}
        }
        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido();
            if (!existeVariable(getContenido()))
            {
                throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) + 1);
                }
            }
            else
            {
                match("--");
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) - 1);
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
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch (operador)
            {
                case "==":
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default:
                    return e1 != e2;
            }

        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
            }
            if (getContenido() == "else")
            {
                //requerimiento 4
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf);
                }
                else
                {
                    Instruccion(!validarIf);
                }
            }
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
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if (evaluacion)
                {
                    Console.Write(resultado);
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
                log.Write(Operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (Operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
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
                log.Write(Operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (Operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }

                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    throw new Error("No existe la variable <" + getContenido() + ">en la linea: " + linea, log);
                }
                log.Write(getContenido() + " ");
                //Requerimiento 1 
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));//stack.Push(float.Parse(getContenido()));
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
                    //tengo dudas de si funciona y como probarlo
                    
                    dominante = casteo;
                    float cast = stack.Pop();
                    switch (dominante)
                    {
                        case Variable.TipoDato.Char:
                            stack.Push((char)cast % 255);
                            break;
                        case Variable.TipoDato.Int:
                            stack.Push((int)cast % 65535);
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