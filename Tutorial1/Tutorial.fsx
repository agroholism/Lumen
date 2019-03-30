let rec fact n =
    match n with
    | 0 -> 1
    | _ -> n * fact (n - 1)
    
let head = 
    function 
    | x::_ -> x
    | [] -> ()

type X = Y | Z of int | A

let main () =
    let z = X.Z 6

    match z with 
    | Y -> printf "Y"
    | Z a -> printf "%i" a
    | o -> printf "o"

    printf "%i" (fact 5)
    System.Console.ReadKey ()

main ()