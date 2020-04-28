grammar Stellaris;

content: 
   expr+
   ;

expr: 
   key op val
   ;

key: 
   id | attrib
   ;

val: 
   id | attrib | group
   ;

op:
    '=' | '>' | '<' | '>' '='| '<' '='
    ;

attrib: 
   id accessor (attrib | id)
   ;

accessor: 
   '.' | '@' | ':'
   ;

group: 
   '{' (expr* | id*) '}'
   ;

id: 
   IDENTIFIER | STRING | INTEGER
   ;

IDENTIFIER: 
   IDENITIFIERHEAD IDENITIFIERBODY*
   ;

INTEGER: 
   [+-]? INTEGERFRAG
   ;

fragment INTEGERFRAG: 
   [0-9]+
   ;

fragment IDENITIFIERHEAD: 
   [a-zA-Z]
   ;

fragment IDENITIFIERBODY
   : IDENITIFIERHEAD | [0-9_]
   ;

STRING: 
   '"' ~["\r\n]* '"'
   ;

COMMENT: 
   '#' ~[\r\n]* -> channel(HIDDEN)
   ;

SPACE: 
   [ \t\f] -> channel(HIDDEN)
   ;

NL: 
   [\r\n] -> channel(HIDDEN)
   ;
