let rec fact n =
    match n with
    | 0 -> 1
    | _ -> n * fact (n - 1)
    
let head = 
    function 
    | x::_ -> Some(x)
    | [] -> None

type X = Y | Z of int | A

let main () =
    let z = X.Z 6

    let z = "d"

    [1; 2; 3; 4]
    |> List.fold (+) 0
    |> ignore

    let v = Some 9
    
    printf "%i" (fact 5)
    System.Console.ReadKey ()

main ()