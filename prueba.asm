;Mario Valdez Rico
;Archivo: prueba.cpp
;Fecha: 14/11/2022 01:05:11 a. m.
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
inicioFor0:
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
MOV AX, 20
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE finFor0
PRINT "hola"
JMP inicioFor0
finFor0:
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
