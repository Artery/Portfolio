(*Das untenstehende OCaml Beispiel ist zur Überprüfung der Korrektheit von AVL-Bäumen (https://de.wikipedia.org/wiki/AVL-Baum)
Es ist recht kompliziert auf den ersten Blick, aber ich finde meine Variante als recht elegant und effizient,
da jeder Knoten im Baum nicht mehr als einmal besucht wird.
Die ersten Deklarationen und Funktionen sind nur Hilfs-Funktionen.
Die eigentliche Funktion beginnt ab Zeile 73
Unten eingefügt befindet sich auch noch ein Beispiel Baum, um das ganze zu testen.*)

(*Typen Deklaration eines AVL-Baumes
Er besteht aus einem leeren Leaf (Blatt) oder einer Node, welcher durch den Typen avl_tree_node repräsentiert wird.
avl_tree_node speichert einen Int-Wert, seinen Balance-Faktor und jeweils einen linken und rechten Teilbaum
Der Balance-Faktor gibt, welche Seite eines Baumes tiefer ist.
	-1 bedeutet die linke Seite ist um eine Stufe tiefer als die rechte
	 2 bedeutet die rechte  Seite ist um zwei Stufen tiefer als die linke
	 0 bedeutet beide Seiten sind gleich tiefer*)
type avl_tree =
    Node of avl_tree_node
  | Leaf
and avl_tree_node = 
{ 
  key : int;
  balance : int;
  left : avl_tree;
  right : avl_tree;
}

(*Hilfs-Funktionen zum Vergleich des Schlüssel am aktuellen Knoten mit dem seines Parent-Knoten, 
  um auf inkonsistente Werte zu testen *)
let less_eq = fun x y -> x <= y;;
let greater_eq = fun x y -> x >= y;;
let equals 	= fun x y -> x =  y;;

(*Hilfs-Funktion, um die Validierungs-Informationen der linke 
und rechten Seite eines Subtrees zu einem Tupel zusammen zu fassen. 
Darüber hinaus wird der Balance-Faktor des aktuellen Knotens berechnet*)
let merge_validation_tupels_with_balance lhs_tupel rhs_tupel = 
	match lhs_tupel,rhs_tupel with
	(lhs_depth, is_lhs_valid), (rhs_depth, is_rhs_valid) -> 
		((Pervasives.max lhs_depth  rhs_depth), (*1*)is_lhs_valid && is_rhs_valid, rhs_depth-lhs_depth)
;;

(*Hilfs-Funktion zum Überprüfen der Korrektheit des Balance-Faktors des aktuellen Knotens
Der berechnete Balance-Faktor muss immer gleich dem sein, der schon im Knoten steht
und er darf nicht größer als 1 bzw. kleiner als -1 sein, ansonsten ist der Baum nicht korrekt.*)
let check_balance cur_balance calcd_balance = 
	(*4.*)cur_balance = calcd_balance && (*5*)abs cur_balance <= 1
;;

(*(Haupt-)Funktion zur Überprüfung eines AVL-Baumes, ob er korrekt ist
Ein AVL-Baum ist dann korrekt, wenn alle der folgenden Bedingungen erfüllt sind:
 1. Alle Teilbäume sind valide
 2. Alle Schlüssel im linken Teilbaum sind höchstens so groß wie der Schlüssel des Wurzelknotens
 3. Alle Schlüssel im rechten Teilbaum sind mindestens so groß wie der Schlüssel des Wurzelknotens 
 4. Die Balancierung des Baumes wird korrekt im Feld balance gespeichert
 5. Die Balancierung ist ein Wert zwischen -1 und 1
 
 Die Idee dieser Implementation ist die, dass jeder Knoten nur ein einziges Mal besucht wird 
 und dann alle Bedingungen auf einmal überprüft werden,  um die Methode möglichst effizient zu gestalten.
 
 Die grundsätzliche Funktionsweise ist folgende:
 A: Wenn der Schlüssel des aktuellen Knotens valide ist, also die 2. bzw. 3. Bedingung erfüllt, 
 B: dann wird für jeden Knoten die innere Validierungs-Funktion rekursiv für jeweils seinen linken und rechten Knoten aufgerufen
 C: Dies wird solange rekursiv weitergeführt, bis jeweils das Ende eines Teilbaums erreicht wird, was durch ein Leaf (Blatt) signalisiert wird
 D: Nun wird angefangen beim Blatt bei jedem Schritt ein Tupel bestehend aus einem Int- und einem Boolean-Wert zurückgegeben
	- Der Int-Wert gibt die aktuelle Tiefe des Subtrees an (begonnen bei 0) und der Boolean-Wert, ob der Subtree valide ist
 E: An jedem Knoten werden nun die beiden Rückgabe-Tupel des linken und rechten Teilbaums zu einem verrechnet (siehe merge_validation_tupels_with_balance) 
	und anschließend die Bedingungen 1., 4. und 5. überprüft
	F: Dies geschieht folgendermaßen: 
	  Sind der linke und rechte Teilbaum valide (der Boolean-Wert ihres Rückgabe-Tupel ist true) und ist 
	  Anhand der beiden Tiefen-Werte der linken und rechten Teilbäume wird der Balance-Faktor berechnet, 
	  ist dieser identisch mit dem gespeicherten Wert und ist er ein valider Balance-Wert (Int-Wert zwischen -1 und 1)
 G: Dies wird solange getan, bis die Rekursion wieder zum Ursprungsknoten zurückgekehrt ist, welcher das finale Tupel zurückgibt
 H: Aus diesem Tupel wird zu letzt der Boolean-Wert extrahiert und von der Haupt-Funktion zurückgegeben, welcher nun angibt, ob der übergebene AVL-Baum korrekt ist*)
let valid_avl avl_tree =
	match (let rec inner_valid subtree parent_key key_check_function = 
		match subtree with
		Node(node) ->
			(*A*)if (*1./2.*)(key_check_function node.key parent_key)
				then 
					match (*E*)(*1*)merge_validation_tupels_with_balance ((*B*)inner_valid node.left node.key less_eq) ((*B*)inner_valid node.right node.key greater_eq) with
					depth, is_tree_valid, calcd_balance -> 
						(*D, hier erfolgt die Tupel-Rückgabe für Nodes*)
						(depth+1, (*F*)((*4.&5.*)check_balance node.balance calcd_balance) && (*1*)is_tree_valid)
			else
				(0, false)
		| (*C*)Leaf -> (*D*)(0,true)
	in (*G*)inner_valid avl_tree (match avl_tree with Node(root) -> root.key) equals) with
	depth, is_tree_valid -> (*H*)is_tree_valid
;;

(*Zum Testen
PS: try.ocamlpro.com ;)
*)
let rec insert new_key new_balance tree	=
	match tree with
	Node(btn) ->
		if new_key <= btn.key
		then Node({key = btn.key; balance = btn.balance; left = (insert new_key new_balance btn.left); right = btn.right})
		else Node({key = btn.key; balance = btn.balance; left = btn.left; right = (insert new_key new_balance btn.right)})
	| Leaf -> Node({key = new_key; balance = new_balance; left = Leaf; right = Leaf})
;;

let tree = Node({key = 10; balance = 1; left = Leaf; right = Leaf});;
let tree = insert 5 (-1) tree;;
let tree = insert 8 (0) tree;;
let tree = insert 3 (-1) tree;;
let tree = insert 2 (0) tree;;
let tree = insert 100 (1) tree;;
let tree = insert 50 (1) tree;;
let tree = insert 60 (0) tree;;
let tree = insert 200 (-1) tree;;
let tree = insert 300 (0) tree;;
let tree = insert 150 (0) tree;;
let tree = insert 140 (0) tree;;
let tree = insert 160 (0) tree;;

valid_avl tree;;