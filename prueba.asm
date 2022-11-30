;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 30/11/2022 12:20:42 a. m.
#make_COM#
include emu8086.inc
ORG 100h
;Variable
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	cinco DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
	k DW ?
PRINT 'Introduce la altura de la piramide: '
CALL SCAN_NUM
MOV altura, CX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if0
MOV AX, altura
PUSH AX
POP AX
MOV i, AX
inicioFor0:
MOV AX, i
PUSH AX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE finFor0
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
inicioWhile0:
MOV AX, j
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile0
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
PRINT '*'
JMP else1
if1:
PRINT '-'
else1:
MOV AX, j
ADD AX, 1
MOV j, AX
JMP inicioWhile0
finWhile0:
PRINTN 
MOV AX, i
SUB AX, 1
MOV i, AX
JMP inicioFor0
finFor0:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
inicioDo0:
PRINT '-'
MOV AX, k
ADD AX, 2
MOV k, AX
MOV AX, k
PUSH AX
MOV AX, altura
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finDo0
JMP inicioDo0
finDo0:
PRINTN 
JMP else0
if0:
PRINTN 
PRINT 'Error: la altura debe de ser mayor que 2'
PRINTN 
else0:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if2
PRINT 'Esto no se debe imprimir'
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if3
PRINT 'Esto tampoco'
if3:
if2:
MOV AX, 258
PUSH AX
POP AX
MOV a, AX
PRINT 'Valor de variable int 'a' antes del casteo: '
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, a
PUSH AX
POP AX
MOV AL, 0
PUSH AX
POP AX
MOV y, AX
PRINTN 
PRINT 'Valor de variable char 'y' despues del casteo de a: '
MOV AX, y
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN 
PRINT 'A continuacion se intenta asignar un int a un char sin usar casteo: '
PRINTN 
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
