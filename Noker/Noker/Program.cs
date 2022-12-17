// mcs -r:LibreriaBaraja.dll Noker.cs
using LibreriaBaraja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Noker{
	public class Noker{
		static void Main(String[] args){
			int n = 5;
			int dinero = 7;
			int apuesta = 0;
			int estrellas = 0;
			String re;
			bool jugar = true;
			bool gana;
			// Bucle para volver a jugar tantas veces que se desee
			// Instrucciones
			Inicio();
			while(jugar){
				Console.WriteLine("\n\tJuego\n");

				// Creación de variables, manos y baraja. Cada juego se crea una nueva
				int contador, puntos1, puntos2, esn, multiplicador = 0;
				
				List<Carta> mano1 = new List<Carta>(n);
				List<Carta> mano2 = new List<Carta>(n);
				Baraja baraja = new Baraja();
				baraja.RellenaBaraja();
				baraja.MezclaBaraja();

				// -- Inicio del juego --
				// Reparto n cartas a cada mano
				Reparte(baraja, mano1, mano2, n);
				Ordena(mano1, n);
				Ordena(mano2, n);
				Console.Clear();
				Ia(mano2, baraja);
				// Se apuesta un valor.
				apuesta = Apostar(dinero, apuesta, mano1, estrellas);
				Console.WriteLine("Esta es tu mano, escribe la posición de carta que desees descartar:");
				MuestraNumerado(mano1);
				contador = Descarte(mano1, n);
				// Compruebo si el contador de descarte es mayor a 0, si no, no entra en el método restock para volver a completar la mano.
				if(contador > 0)
					Restock(contador, mano1, baraja);
				// Muestro la mano final, con la cual se jugará.
				Console.WriteLine("\nTu mano es la siguiente: ");
				Muestra(mano1);
				Console.WriteLine("Intro para ver las cartas de la mesa: ");
				Console.ReadLine();
				// Cuento los puntos de ambas manos.
				puntos1 = CuentaPuntos(mano1, n);
				puntos2 = CuentaPuntos(mano2, n);
				// Muestro ambas manos y manos y declaro el ganador o empate.
				Console.WriteLine("");
				Console.WriteLine("Tu mano:");
				Muestra(mano1);
				Console.WriteLine("Mano del contrincante:");
				Muestra(mano2);
				gana = CompruebaGanador(puntos1, puntos2);
				if (gana)
				{
					switch (puntos1)
					{
						case 2: multiplicador = 1;
							break;
						case 4: multiplicador = 2;
							break;
						case 6: multiplicador = 3;
							break;
						case 8: multiplicador = 4;
							break;
						case 12: multiplicador = 5;
							break;
					}
					apuesta = (apuesta * multiplicador);
					Console.WriteLine("\n¡Felicidades! Has conseguido "+ apuesta +" monedas");
					dinero = (dinero + apuesta);
				}
				else
				{
					Console.WriteLine("\n¡Oh no! Has perdido "+ apuesta +" monedas");
					dinero = (dinero - apuesta);
				}
				Console.WriteLine("Ahora tienes "+ dinero +" monedas");
				Extra(dinero, estrellas);
				if (dinero <= 0)
				{
					Console.WriteLine("Te quedaste sin monedas.\n\tHas perdido");
					jugar = false;
					Console.WriteLine("\nGracias por jugar");
				}
				else
				{
					Console.WriteLine("\n\t-- Fin del programa. Rejugar?(1/0)[1] --");
					if (estrellas == 999)
					{
						jugar = false;
					}
					else
					{
						re = Console.ReadLine();
						if(int.TryParse(re, out esn)){
							esn = int.Parse(re);
							if(esn == 1)
								jugar = true;
							else if(esn == 0){
								jugar = false;
								Console.WriteLine("\nGracias por jugar");
							}
							else{
								jugar = true;
								Console.WriteLine("\n\tjuego de nuevo\n");

							}
						}
						else{
							Console.WriteLine("\n\tjuego de nuevo\n");
						}
					}
					
				}
				
			}

		}
		
		// Método que explica las regals básicas del juego.
		public static void Inicio(){
			Console.WriteLine("////////////////////////////////////");
			Console.WriteLine("//\tBienvenidos a Noker\t  //");
			Console.WriteLine("////////////////////////////////////\n");
			Console.WriteLine("Clasificación de jugadas: ");
			Console.WriteLine("nº5: Noker");
			Console.WriteLine("nº4: Full");
			Console.WriteLine("nº3: Trio");
			Console.WriteLine("nº2: Dobles parejas");
			Console.WriteLine("nº1: Parejas");
			Console.WriteLine("\nEl Noker únicamente hace uso de los símbolos, así que para este juego tenlos en cuenta, y no los números.");
			Console.WriteLine("La apuesta mínima es 3, y va aumentando conforme tus ahorros aumenten.");
			Console.WriteLine("Si ganas, se te devolverá tu apuesta multiplicada por el valor de la jugada (Del 1, el más bajo al 5, el más alto).");
			Console.WriteLine("\tPresiona cualquier intro para continuar.");
			Console.ReadLine();
		}
		
		// Método que añade cartas a las listas mano.
		public static void Reparte(Baraja baraja, List<Carta> mano1, List<Carta> mano2, int n){
			Carta c;
			// Añado n cartas a las mano 1 y 2. 
			for(int i=0; i<n; i++){
				c = baraja.PideCarta();
				mano1.Add(c);
			}
			for(int i=0; i<n; i++){
				c = baraja.PideCarta();
				mano2.Add(c);
			}
		}

		// Método que ordena las cartas de la mano
		public static void Ordena(List<Carta> mano, int n){
			Carta aux;
			// Esto ordena las cartas por palos, pero no pero jugadas.
			for(int i = 0; i < n; i++){
				for(int j = 0; j < n-1; j++){
					if(mano[i].getPalo() < mano[j].getPalo()){
						aux = mano[i];
						mano[i] = mano[j];
						mano[j] = aux;
					}
				}
			}
		}

		// Método que establece la apuesta retornando el valor que se apostará
		public static int Apostar(int dinero, int apuesta, List<Carta>mano1, int estrellas)
		{
			
			String s;
			int min = 3;
			if (dinero < min)
				min = dinero;
			else if (dinero > 15 && dinero < 25)
				min = 5;
			else if (dinero > 25 && dinero < 35)
				min = 7;
			else if (dinero > 35 && dinero < 50)
				min = 15;
			else if (dinero > 50 && dinero < 75)
				min = 25;
			else if (dinero > 75 && dinero < 100)
				min = 40;
			else if (dinero > 100)
				min = 50;
			
			
			Console.WriteLine("\n\t# Dinero: "+dinero+" #\t\t@ Estrellas[★]: "+estrellas+" @");
			Muestra(mano1);
			Console.WriteLine("Esta es tu mano inical, introduce el número de monedas que quieras apostar[min "+min+"]");
			s = Console.ReadLine();
			if(int.TryParse(s, out apuesta)){
				apuesta = int.Parse(s);
				// Try catch para que los datos introducidos sea mayor al mínimo obligatoriamente, en otro caso se cerrará el modo apuesta.
				if (apuesta < min || apuesta > dinero)
				{
					Console.WriteLine("Datos incorrectos, apuestas el mínimo ["+min+"]");
					apuesta = min;
				}
				
			}
			else{
				Console.WriteLine("Apuestas el mínimo["+min+"]");
				apuesta = min;
			}
			
			dinero = dinero - apuesta;
			Console.WriteLine("Has apostado "+apuesta+" monedas");
			Console.WriteLine("\n\t# Dinero: "+dinero+" # ");
			return apuesta;
		}
		
		// Método que muestra las cartas de la mano de forma numerada, de manera que sea mucho más facil la selección de cada una.
		public static void MuestraNumerado(List<Carta> mano){

			Console.WriteLine(" Carta  1 Carta  2 Carta  3 Carta  4 Carta  5 ");
			Console.Write(Carta.MuestraCartasHorizontal(mano));			
		}
		
		// Método que muestra las cartas de una mano.
		public static void Muestra(List<Carta> mano){

			Console.Write(Carta.MuestraCartasHorizontal(mano));			
		}
		
		// Método que quita las cartas deseadas de una Lista. Retorna el número de cartas que se han descartado.
		public static int Descarte(List<Carta> mano, int n){
			// Creo variables y bucle para comprobar datos y que se repita si el dicho dato no es correcto.
			int sel;
			String s;
			int contador = 0;
			bool entra = true;
			Console.WriteLine("Elige las cartas que quieras descartar seleccionando su posición e intro. Para finalizar la selección de descarte presiona 0: ");
			while(entra)
			{
				Console.Write("Descarto la carta nº[0]: ");
				s = Console.ReadLine();
				if (s == "")
				{
					entra = false;
				}
				else
				{
					if(int.TryParse(s, out sel)){
						sel = int.Parse(s);
						// Try catch para que los datos introducidos sean del 0 al 5 obligatoriamente, en otro caso se cerrará el modo descarte.
						try
						{
							// Borro la posición enviada.
							mano.RemoveAt(sel - 1);
							// Sumo uno al contador de descarte.
							contador++;
							// Vuelvo a mostrar la mano con las cartas restantes.
							MuestraNumerado(mano);
						}
						catch (Exception e)
						{
							entra = false;
						}
					}
					else
					{
						entra = false;
					}
				}
				if (!entra)
				{
					Console.WriteLine("-- Fin del descarte --");
				}
			}
			return contador;
		}
		
		// Método que recibe el número de cartas que se han descartado para volver a meter tantas cartas como el número que recibe.
		public static void Restock(int n, List<Carta> mano, Baraja baraja){
			Carta c;
			// Añado a la mano tantas cartas como el int que recibe. 
			for(int i=0; i<n; i++){
				c = baraja.PideCarta();
				mano.Add(c);
			}
		}
		
		// Método que comprueba el ganador en base al total de puntos que ha conseguido cada mano, comparándolos entre si.
		public static bool CompruebaGanador(int puntos1, int puntos2)
		{
			bool gana = false;
			// Switches que dependiendo de el número de puntos muestra la jugada que tiene cada mano.
			switch(puntos1){
				case 2: Console.WriteLine("Tienes una pareja ");break;
				case 4: Console.WriteLine("Tienes dobles parejas ");break;
				case 6: Console.WriteLine("Tienes un trío ");break;
				case 8: Console.WriteLine("Tienes un Full ");break;
				case 12: Console.WriteLine("Tienes un Noker ");break;
			}

			switch(puntos2){
				case 2: Console.WriteLine("Tu contrincante tiene una pareja ");break;
				case 4: Console.WriteLine("Tu contrincante tiene dobles parejas ");break;
				case 6: Console.WriteLine("Tu contrincante tiene un trío ");break;
				case 8: Console.WriteLine("Tu contrincante tiene un Full ");break;
				case 12: Console.WriteLine("Tu contrincante tiene un Noker ");break;
			}
			// Condición que imprime si el ganador es el jugador, contrincante o empate según quién tenga más puntos 
			if(puntos1>puntos2){
				Console.WriteLine("\nNokeaste al contrincante\n\t¡Has ganado esta mano!");
				gana = true;
			}
			else if(puntos1 == puntos2){
				Console.WriteLine("\nEl contrincante y tu os habeis noqueado mutuamente\n\t¡Empate!");
			}
			else{
				Console.WriteLine("\n¡El contrincante te ha nokeado!\n\tHas perdido esta mano");

			}
			return gana;
		}
		
		// Método que cuenta los puntos conseguidos de una mano. Retorna dichos puntos.
		// Dependiendo de la cantidad de puntos, la mano tendrá una jugada u otra.
		public static int CuentaPuntos(List<Carta> mano, int n){
			// Declaración de la variable a -5, ya que como mínimo cada mano tendrá 5 puntos asegurados (que será ella misma).
			int puntos = -n;
			// Dos bucles en los que se comparan cada carta de una mano entre sí. Si la carta coincide se suma un punto.
			for(int i=0; i < n; i++){
				for(int j=0; j < n; j++){
					if(mano[i].getPalo() == mano[j].getPalo()){
						puntos++;
					}
				}
			}
			return puntos;
		}

		public static void Ia(List<Carta> mano, Baraja baraja)
		{
			int puntos = 0;
			int posicion = 0;
			int contador = 0;
		
			for(int i=0; i < mano.Count; i++){
				for(int j=0; j< mano.Count; j++){
					if(mano[i].getPalo() == mano[j].getPalo())
					{
						puntos++;
						posicion = j;
					}
				}
				if (puntos == 1)
				{
					mano.RemoveAt(posicion);
					contador++;
				}
				puntos = 0;
			}
			Restock(contador, mano, baraja);
		}

		public static void Extra(int dinero, int estrellas)
		{
			switch (dinero)
			{
				case 72: Console.Clear();Console.WriteLine("ou iea 72");
					for (int i = 0; i < 72; i++)
					{
						Console.Write("72 ");
					}
					Console.WriteLine("\n");
					break;
				case 666: Console.Clear();Console.WriteLine("Epa el devil");
					break;
			}
			if (dinero >= 500)
			{
				Console.Clear();
				Console.WriteLine("¡Felicidades! Has conseguido una estrella, toma[★].\nEl dinero volverá a 7 monedas.");
				dinero = 7;
				estrellas++;
				switch (estrellas)
				{
					case 1: Console.WriteLine("Ahora deberías dejar de jugar.");
						break;
					case 2: Console.WriteLine("En serio, mejor para.");
						break;
					case 3: Console.WriteLine("¿Pero qué estás haciendo con tu vida? Cierra el juego y llama a tu madre.");
						break;
					case 4: Console.WriteLine("Último intento, esto no sirve para nada.");
						break;
					case 5: Console.Clear();
						estrellas = 999;
						break;
				}
			}
		}
	}
}
