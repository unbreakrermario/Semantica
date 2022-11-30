;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 29/11/2022 10:59:54 p. m.
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
inicioDo0:
MOV AX, i
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN 
INC i
MOV AX, i
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finDo0
JMP inicioDo0
finDo0:
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
