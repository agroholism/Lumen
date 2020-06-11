let main () =
    let a = [| 5; 23; 5; 6; 2; 3; 5 |]
    for i in a do
        System.Console.WriteLine i
    System.Console.ReadKey ()

main ()