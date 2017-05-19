start:
mov ah,1
mov al,2
mov bh,3
mov bl,4
mov ch,5
mov cl,6
mov dh,1
mov dl,2
inc al
add ah,5
sub bl,4
mov bl,5
stc
subb bl,4
je metka
jmp start
metka:
mul dl,5
shl ch,3
shr ch,2
ror ch,6
rol ch,5
stc 
rrc ch,3
rlc ch,2
div dl,3
mov al,5
add al,121
add al,1
add al,1
sub al,127
sub al,127
mov al,13
mul al,al
div al,al
xor al,al
mov ah,5
mov cl,117
and ah,3
or ah,cl
not cl
stc
mov al,120
subb al,127
js metka2
jmp start
metka2:
mov ch,20
mov al,1
metka3:
inc al
loop metka3
cmp al,10
jg metka4
jmp start
metka4:
swap al
test al,45
neg al
dec al
mov #7,15
mov ah,#7
mov #5,37
mov ch,5
cld
movs #15,#7
std
sto
sts
stz
stc
sti
cld
clo
cls
clz
clc
cli
int 1
sti
int 1
mov dl,3
mov #dl,146
ret
int1
mov al,15
iret