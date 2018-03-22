module Peano

type Number = Zero | Successor of Number

let one   = Successor Zero
let two   = Successor one
let three = Successor two
let four  = Successor three
let five  = Successor four
let six   = Successor five
let seven = Successor six
let eight = Successor seven
let nine  = Successor eight
let ten   = Successor nine

let increment = Successor

let decrement x =
    match x with
    | Zero -> invalidOp "Can't subtract from zero"
    | Successor x' -> x'

let rec add x y =
    match x with
    | Zero -> y
    | Successor x' -> add x' (Successor y)

let rec subtract x y =
    match (x, y) with
    | (_, Zero) -> x
    | (Zero, _) -> invalidOp "Can't subtract a larger number from a smaller number"
    | (Successor x', Successor y') -> subtract x' y'

let multiply x y =
    let rec multiply' a x y =
        match x with
        | Zero -> a
        | Successor x' -> multiply' (add a y) x' y
    in multiply' Zero x y

let rec lessThan x y =
    match (x, y) with
    | (_, Zero) -> false
    | (Zero, _) -> true
    | (Successor x', Successor y') -> lessThan x' y'

let divide x y =
    if y = Zero then
        invalidOp "Can't divide by zero"
    else
        let rec divide' a x y =
            if lessThan x y then a
            else divide' (increment a) (subtract x y) y
        in divide' Zero x y

let rec modulo x y =
    if y = Zero then invalidOp "Can't divide by zero"
    else if lessThan x y then x
    else modulo (subtract x y) y

let printDigit d =
    if      d = Zero  then "0"
    else if d = one   then "1"
    else if d = two   then "2"
    else if d = three then "3"
    else if d = four  then "4"
    else if d = five  then "5"
    else if d = six   then "6"
    else if d = seven then "7"
    else if d = eight then "8"
    else if d = nine  then "9"
    else invalidOp "Can't print digit for value higher than nine"

let print x =
    if x = Zero then "0"
    else
        let rec print' s n =
            if n = Zero then s
            else 
                let d = modulo n ten in
                print' (printDigit d + s) (divide n ten)
        in print' "" x

let rec parse s =
    match s with
    | "0" -> Zero
    | "1" -> one
    | "2" -> two
    | "3" -> three
    | "4" -> four
    | "5" -> five
    | "6" -> six
    | "7" -> seven
    | "8" -> eight
    | "9" -> nine
    | _   ->
        let lastIndex = s.Length - 1 in
        let last = s.[lastIndex].ToString() in
        let rest = s.[0..lastIndex - 1] in
        let lastValue = parse last in
        let restValue = parse rest in
        add lastValue (multiply ten restValue)
