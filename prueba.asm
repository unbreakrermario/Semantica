;Archivo: prueba.cpp
;Fecha: 26/10/2022 09:58:24 p. m.
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
	k DW ?
	l DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
inicioFor0:
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor1:
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
POP AX
MOV x, AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
POP AX
MOV x, AX
finFor1:
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor2:
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX, 1
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
POP AX
MOV x, AX
finFor2:
finFor0:
RET
END
