=begin
Folgende Annahme:
Es existiert eine unbestimmte Anzahl von Assets (technischen Anlagen). Jedes dieser Assets besteht aus 1 bis n Sub-Assets.
Jedes Sub-Asset hat entweder 1 ODER 4 verschiedene Zeitreihen, welche Auskunft über verschiedene technische Informationen geben, in Form von float-Werten.

Die untenstehende Methode soll ein 1D-float-Array zurückgeben, in dem für ein Asset pro Zeitschritt ein Wert enthalten ist.
Wird zum Beispiel ein Zeitraum betrachtet, welcher 5 Zeitschritte lang ist, dann könnte eine mögliche Rückgabe folgendermaßen aussehen: [1.0, 2.0, 3.0, 4.0, 5.0]

Es können folgende 3 Fälle eintreten:
1. Für den Fall, dass ein Asset nur aus einem Sub-Asset besteht und dieses nur 1 Zeitreihe besitzt, kann diese einfach zurückgeben werden.
2. Für den Fall, dass ein Asset nur aus einem Sub-Asset besteht und dieses 4 Zeitreihen besitzt, müssen diese Zeitreihen zu einer "kombiniert" werden.
3. Für den Fall, dass ein Asset aus mehreren Sub-Assets besteht und diese jeweils 4 Zeitreihen besitzen, müssen die Zeitreihen zu einer pro Sub-Asset und 
	anschließend zu einer "gesamt" Zeitreihe "kombiniert" werden

Die Herausforderung an diesem Problem war, dass es festgelegt war, wie die Zeitreihen-Daten übergeben werden (1D-float-Array) und wie der Output aussehen soll (ebenfalls 1D-float-Array).
Die Werte aus dem 1D-float-Array mussten nun möglichst effektiv und kompakt kombiniert werden.
Die ganze Methode ist recht kompliziert im Detail, was ein Manko ist. Aber ich finde sie ist eine ziemlich coole Variante mit relativ wenigen Zeilen ans Ziel zu kommen, 
was sie zu der kompaktesten Variante macht, die ich gefunden habe.

=end

#Methode kompakt, nur kurz kommentiert:
#Variante mit Beispielwerten und umfangreich kommentiert auf der nächsten Seite.
  def self.build_pav_array(rts_identifier, from_time, to_time, extended_formular, num_sub_assets, time_series)

	# Anfordern der Zeitreihen-Werte als 1D-float-Array
    pav_array       =   DailyEvent.get_time_series_as_float(rts_identifier, from_time, to_time, time_series)	

    num_timestamps  =   pav_array.count / (num_sub_assets * time_series.count)
    pav_array       =   pav_array.each_slice(num_timestamps).to_a

    if extended_formular
	  # Kombinieren der Zeitreihen-Werte für jeden Zeitschritt zu einem Wert pro Zeitschritt und Sub-Asset
      pav_array     =   pav_array.each_slice(time_series.count).to_a.
							map{|ses_chunk, pav_chunk, avc_chunk, rl_chunk| pav_chunk.zip(rl_chunk, ses_chunk, avc_chunk).
								map{|pav_element, rl_element, ses_element, avc_element| (pav_element + rl_element * ses_element ) * avc_element}}   
    end

	# Zusammenfassen der Werte jedes Sub-Asset zu einem Wert pro Zeitschritt
    return  pav_array.transpose.map {|element| element.reduce(:+)}
  end

#Das untenstehende Beispiel ist für den kompliziertesten Fall 3.
#Beispielhafte Werte:
#	input: 	rts_identifier 		=> 	"Asset1"
# 			from_time 			=> 	t+0 (vereinfacht, normalerweise als Datetime z.B. 01.01.2016 00:00 +0100)
# 			to_time 			=> 	t+2 (vereinfacht, normalerweise als Datetime z.B. 01.01.2016 00:03 +0100)
# 			extended_formular 	=> 	true
# 			num_sub_assets 		=> 	2
# 			time_series 		=> 	[SES, PAv, AvC, RL]
#	output:	[8.0, 8.2, 8.4]
def self.build_pav_array(rts_identifier, from_time, to_time, extended_formular, num_sub_assets, time_series)

	# Anfordern der Daten als float-Array
	# Die Daten kommen immer als ein großes 1D-Array zurück und sind nach Sub-Asset und nach Zeitreihen-Typ(beide absteigend) sortiert 
	# Die "Herausforderung" war es an dieser Stelle das Array "korrekt" zu bearbeiten, da die Daten nicht anders "beschafft" werden können
	# SES, PAv, AvC und RL sind die Bezeichnungen der Zeitreihen, ihre Bedeutung ist nicht relevant für dieses Beispiel
    pav_array       =   DailyEvent.get_time_series_as_float(rts_identifier, from_time, to_time, time_series)	
	#	  | Sub-Asset1												|| Sub-Asset2												|
	#	  |t+0 t+1  t+2| t+0  t+1  t+2| t+0  t+1  t+2| t+0  t+1  t+2||t+0  t+1  t+2| t+0  t+1  t+2| t+0  t+1  t+2| t+0  t+1  t+2|
	# 	  |    SES	   |      PAv	  |	     AvC	 |	    RL		||	   SES	   |	  PAv	  |		 AvC	 |		RL		|
	# => [0.0, 0.0, 0.0, 5.0, 5.1, 5.2, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 3.0, 3.1, 3.2, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0]
	
	# Zu Testzwecken einfach die obere Zeile auskommentieren und die Zeile unten benutzen:
	# pav_array 	=	[0.0, 0.0, 0.0, 5.0, 5.1, 5.2, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 3.0, 3.1, 3.2, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0]
	
	
	# Hier wird die Anzahl der Werte pro Zeitreihe ermittelt.
	# Jede Zeitreihe hat zwar gleich viele Elemente, aber allein aus der from_time und to_time lässt sich die Anzahl nicht ablesen.
	# Da die Auflösung der Zeitschritte unterschiedlich sein kann, z.B. minutenweise oder auch stundenweise (ein Wert pro Minute bzw. ein Wert pro Stunde)
    num_timestamps  =   pav_array.count / (num_sub_assets * time_series.count)
	# => 3
	
	# Es wird nun jede Zeitreihe in einen eigenen Block/Chunk "zerschnitten"
	# Da jede Zeitreihe in diesem Beispiel 3 Werte beinhaltet, werden 3er Blöcke/Chunks gebildet
    pav_array       =   pav_array.each_slice(num_timestamps).to_a
	#	  | Sub-Asset1												        || Sub-Asset2														|
	#	  |t+0  t+1  t+2|   t+0  t+1  t+2 |  t+0  t+1  t+2|   t+0  t+1  t+2 || t+0  t+1  t+2|   t+0  t+1  t+2 |  t+0  t+1  t+2|   t+0  t+1  t+2 |
	# 	  |     SES	    |        PAv	  |	      AvC	  |	       RL		||	    SES	    |	     PAv	  |		  AvC	  |		   RL		|
    # => [[0.0, 0.0, 0.0], [5.0, 5.1, 5.2], [1.0, 1.0, 1.0], [0.0, 0.0, 0.0], [0.0, 0.0, 0.0], [3.0, 3.1, 3.2], [1.0, 1.0, 1.0], [0.0, 0.0, 0.0]]

    if extended_formular
      
	  # Nun werden die Zeitreihen-Chunks aufgeteilt in Chunks pro Sub-Asset
      pav_array     =   pav_array.each_slice(time_series.count).to_a
	  #	  	 | Sub-Asset1												        ||  Sub-Asset2														 |
	  #	     |t+0  t+1  t+2|   t+0  t+1  t+2 |  t+0  t+1  t+2|   t+0  t+1  t+2  ||  t+0  t+1  t+2   |t+0  t+1  t+2|   t+0  t+1  t+2|   t+0  t+1  t+2 |
	  #   	 |     SES	   |        PAv	     |	     AvC	 |	      RL		||	     SES	    |	  PAv	  |		   AvC	   |		RL		 |
      # => [[[0.0, 0.0, 0.0], [5.0, 5.1, 5.2], [1.0, 1.0, 1.0], [0.0, 0.0, 0.0]], [[0.0, 0.0, 0.0], [3.0, 3.1, 3.2], [1.0, 1.0, 1.0], [0.0, 0.0, 0.0]]]
	  
	  # Jetzt muss für jeden Zeitschritt ein Wert pro Sub-Asset errechnet werden
	  # Also in unserem Beispiel muss am Ende ein 2D-Array herauskommen
	  # Je ein Wert pro Zeitschritt und pro Sub-Asset also 2x3 Werte ([[5.0, 5.1, 5.2], [3.0, 3.1, 3.2]])
	  # Das Komplizierte hierbei ist, dass die Zeitreihen z.B. nicht einfach aufsummiert werden können, 
	  # sondern mit einer speziellen Formel zusammen gerechnet werden müssen:
	  # Formel: (PAv + RL * SES) * AvC
	  
	  # map ist ähnlich zu einer for-each-loop aus anderen Programmier-Sprachen
	  # Es werden nun zuerst "Zeitschritt-Chunks" gebildet, das bedeutet, je ein Wert aus jeder Zeitreihe eines Sub-Assets wird zu einem Chunk zusammengefasst
	  # Z.B. aus [[SES_1, SES_2], [PAv_1, PAv_2]], ... wird [[SES_1, PAv_1, ...], [SES_2, PAv_2, ...]]
      # Die untenstehenden drei Code-Zeilen sind eine, sie wurden der übersichthalber mit Absätzen getrennt
	  pav_array     =   pav_array.map{|ses_chunk, pav_chunk, avc_chunk, rl_chunk| pav_chunk.
        zip(rl_chunk, ses_chunk, avc_chunk).
	  #	  	 | Sub-Asset1												        || Sub-Asset2													|
	  #		 | 		 t+0		|  |		t+1		  |	 |		t+2		  	|	||		 t+0		|  |		 t+1	  |  |		 t+2		|
	  #   	 |PAv, RL,  SES, AvC|  |PAv, RL,  SES, AvC|  |PAv, RL,  SES, AvC|   ||PAv, RL,  SES, AvC|  |PAv, RL,  SES, AvC|  |PAv, RL,  SES, AvC||   
      # => [[[5.0, 0.0, 0.0, 1.0], [5.1, 0.0, 0.0, 1.0], [5.2, 0.0, 0.0, 1.0]], [[3.0, 0.0, 0.0, 1.0], [3.1, 0.0, 0.0, 1.0], [3.2, 0.0, 0.0, 1.0]]]
	  
	  # Anschließend wird aus den 4 Werten eines "Zeitschritt-Chunks" ein einzelner Wert berechnet
        map{|pav_element, rl_element, ses_element, avc_element| (pav_element + rl_element * ses_element ) * avc_element}}   
      #	  	| Sub-Asset1   || Sub-Asset2   |
	  #		|t+0  t+1  t+2 || t+0  t+1  t+2|
	  #		|   new PAv    ||    new PAv   |
	  # => [[5.0, 5.1, 5.2], [3.0, 3.1, 3.2]]
    end

	# Zu guter Letzt können die Werte der Sub-Assets einfach pro Zeitschritt aufsummiert werden
    return  pav_array.transpose.map {|element| element.reduce(:+)}
	#	 | Sub-Asset1+Sub-Asset2 |
	#	 |t+0   t+1   t+2 |
	#	 |     new PAv    |
    # => [8.0,  8.2,  8.4 ]
  end