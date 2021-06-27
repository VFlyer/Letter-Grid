using System.Collections; //Copyright (c) 2323, Jort Mama, All Rights Reserved.
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterGrid : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMSelectable[] Arrows;
   public TextMesh[] ModuleLetterText;
   public Font[] Fonts;
   public Material[] FontMats;
   public GameObject Morse;
   public GameObject Tap;
   public GameObject More;
   public GameObject Binary;
   public SpriteRenderer Maritime;
   public Sprite[] Flags;
   public Material MorseMat;
   public Material TapMat;
   public Material MoreMat;
   public Material BinaryMat;
   public Material OffMat;

   int[] Chooser = { 0, 1, 2, 3 };
   int[] FontMaterialNumbers = new int[25];
   int CurrentLetter;
   int Placement;
   int tiwmi;

   string[] ValidWords = { "ACTOR", "BRAWL", "CURVE", "DELTA", "EXISH", "FIRED", "GRABS", "HOTEL", "INERT", "KNIFE", "LEMON", "MIGHT", "NYMPH", "OVALS", "PLUMS", "QUOTH", "RAIDS", "SQUAB", "TOXIC", "ULZIE", "VEILS", "WAVED", "XRAYS", "YANKS", "ZONED" };
   string[] EncryptionsName = { "Animals", "Bioblanner", "Boozleglyphs", "Braille", "Decoborders", "Dancing Men", "English", "Hieroglyphs", "Cube Symbols", "Lombax", "Moon Type", "Pigpen", "R'lyehian", "Elder Futhark", "Semaphore", "Semaphore Telegraph", "Standard Galactic Alphabet", "CMP", "Wingdings", "Zoni", "Morse Code", "Tap Code", "More Code", "Binary", "Maritime" };
   string[] Morephabet = { ".-.,.", "=", "=,,", "=,-", ".,.-=", "-.-", ".", ",..", "=--=,", ",", "=-,=", ",,.", "--.=", "-----", "-", "==.", ",--=", "-,==", "=.,,", "=====", "=,", ",=", "=.", "-,", ",=-" };
   string[] MorseLetters = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--.." };
   List<string> Sync = new List<string> { "a", "p'", "C", "t'", "e", "f", "k'", "h", "i", "k", "r'", "m", "n", "o", "p", "?", "r", "s", "t", "u", "f'", "w", "!", "y", "s'" };
   List<string> TempValidWord = new List<string> { };
   List<string> PigpenSwap = new List<string> { "A", "C", "E", "G", "I", "K", "M", "O", "Q", "S", "U", "W", "Y", "B", "D", "F", "H", "L", "N", "P", "R", "T", "V", "X", "Z" };
   string Alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";

   static char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
   char[] GridOfLetters = { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' };
   List<char> UnusedLetters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

   bool[][] BinaryRepresentations = new bool[25][] {
      new bool[5] {false, false, false, false, true}, new bool[5] {false, false, false, true, false}, new bool[5] {false, false, false, true, true}, new bool[5] {false, false, true, false, false}, new bool[5] {false, false, true, false, true}, new bool[5] {false, false, true, true, false}, new bool[5] {false, false, true, true, true}, new bool[5] {false, true, false, false, false}, new bool[5] {false, true, false, false, true}, new bool[5] {false, true, false, true, true}, new bool[5] {false, true, true, false, false}, new bool[5] {false, true, true, false, true}, new bool[5] {false, true, true, true, false}, new bool[5] {false, true, true, true, true}, new bool[5] {true, false, false, false, false}, new bool[5] {true, false, false, false, true}, new bool[5] {true, false, false, true, false}, new bool[5] {true, false, false, true, true}, new bool[5] {true, false, true, false, false}, new bool[5] {true, false, true, false, true}, new bool[5] {true, false, true, true, false}, new bool[5] {true, false, true, true, true}, new bool[5] {true, true, false, false, false}, new bool[5] {true, true, false, false, true}, new bool[5] {true, true, false, true, false}
    };

   static int moduleIdCounter = 1;
   int moduleId;
   private bool moduleSolved;

   void Awake () {
      moduleId = moduleIdCounter++;

      foreach (KMSelectable Arrow in Arrows) {
         Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
      }
   }

   void Start () {
      for (int i = 0; i < 25; i++) {
         FontMaterialNumbers[i] = i;
      }
      ValidWords.Shuffle(); // So it doesn't choose the first one alphabetically
      UnusedLetters.Shuffle();
      Chooser.Shuffle();
      Restart:
      Placement = UnityEngine.Random.Range(0, 25);
      CurrentLetter = UnityEngine.Random.Range(0, 25);
      for (int i = 0; i < 25; i++) {
         if (Letters[CurrentLetter] == ValidWords[i][Placement / 5] || Letters[CurrentLetter] == ValidWords[i][Placement % 5] ||
         Letters[CurrentLetter] == ValidWords[i][4 - (Placement / 5)] || Letters[CurrentLetter] == ValidWords[i][4 - (Placement % 5)]) { //Determines if there is a possible word that has the chosen letter in that position, or the reverse position
            TempValidWord.Add(ValidWords[i]);
         }
      }
      if (TempValidWord.Count() == 0 && tiwmi <= 1000) {
         tiwmi++;
         if (tiwmi == 1000) {
            GetComponent<KMBombModule>().HandlePass();
            return;
         }
         goto Restart;
      }
      //Debug.Log(Letters[CurrentLetter]);
      Debug.LogFormat("[Letter Grid #{0}] A puzzle was generated in {1} attempts.", moduleId, tiwmi + 1);
      Debug.LogFormat("[Letter Grid #{0}] The contained word on the module will be one of the following:", moduleId);
      for (int i = 0; i < TempValidWord.Count(); i++) {
         Debug.LogFormat("[Letter Grid #{0}] {1}", moduleId, TempValidWord[i]);
      }
      int PlacingLetter = 0;
      for (int P = 0; P < 4; P++) { //Randomizes which word is used and how it is placed.
         switch (Chooser[P]) {
            case 0:
               for (int L = 0; L < TempValidWord.Count(); L++) {
                  if (Letters[CurrentLetter] == TempValidWord[L][Placement / 5]) { //Places the word in the same row
                     for (int i = 0; i < 25; i++) {
                        if (i / 5 == Placement / 5) {
                           if (GridOfLetters[i] == '.') {
                              GridOfLetters[i] = TempValidWord[L][PlacingLetter];
                              UnusedLetters.Remove(TempValidWord[L][PlacingLetter]);
                           }
                           PlacingLetter++;
                        }
                     }
                     goto End;
                  }
               }
               break;
            case 1:
               for (int L = 0; L < TempValidWord.Count(); L++) {
                  if (Letters[CurrentLetter] == TempValidWord[L][4 - (Placement / 5)]) { //Places the word in the same row backwards
                     for (int i = 0; i < 25; i++) {
                        if (4 - (i / 5) == 4 - (Placement / 5)) {
                           if (GridOfLetters[i] == '.') {
                              GridOfLetters[i] = TempValidWord[L][4 - PlacingLetter];
                              UnusedLetters.Remove(TempValidWord[L][4 - PlacingLetter]);
                           }
                           PlacingLetter++;
                        }
                     }
                     goto End;
                  }
               }
               break;
            case 2:
               for (int L = 0; L < TempValidWord.Count(); L++) {
                  if (Letters[CurrentLetter] == TempValidWord[L][4 - (Placement % 5)]) { //Places the word in the same column backwards
                     for (int i = 0; i < 25; i++) {
                        if (4 - (i % 5) == 4 - (Placement % 5)) {
                           if (GridOfLetters[i] == '.') {
                              GridOfLetters[i] = TempValidWord[L][4 - PlacingLetter];
                              UnusedLetters.Remove(TempValidWord[L][4 - PlacingLetter]);
                           }
                           PlacingLetter++;
                        }
                     }
                     goto End;
                  }
               }
               break;
            case 3:
               for (int L = 0; L < TempValidWord.Count(); L++) {
                  if (Letters[CurrentLetter] == TempValidWord[L][Placement % 5]) { //Places the word in the same column
                     for (int i = 0; i < 25; i++) {
                        if (i % 5 == Placement % 5) {
                           if (GridOfLetters[i] == '.') {
                              GridOfLetters[i] = TempValidWord[L][PlacingLetter];
                              UnusedLetters.Remove(TempValidWord[L][PlacingLetter]);
                           }
                           PlacingLetter++;
                        }
                     }
                     goto End;
                  }
               }
               break;
         }
      }
      End:
      for (int i = 0; i < 25; i++) {
         if (GridOfLetters[i] == '.') {
            int Whatever = 0;
            Whatever = UnityEngine.Random.Range(0, UnusedLetters.Count());
            GridOfLetters[i] = UnusedLetters[Whatever];
            UnusedLetters.Remove(UnusedLetters[Whatever]);
         }
      }
      string TempLog = "";
      Debug.LogFormat("[Letter Grid #{0}] The grid is:", moduleId);
      for (int i = 0; i < 25; i++) {
         ModuleLetterText[i].text = GridOfLetters[i].ToString();
         TempLog += GridOfLetters[i].ToString();
         if (i % 5 == 4) {
            Debug.LogFormat("[Letter Grid #{0}] {1}", moduleId, TempLog);
            TempLog = string.Empty;
         }
      }

      Reshuffle:
      FontMaterialNumbers.Shuffle();
      for (int i = 0; i < 25; i++) {
         if ((FontMaterialNumbers[i] == 14 && ModuleLetterText[i].text == "N") || (FontMaterialNumbers[i] == 7 && "FX".Contains(ModuleLetterText[i].text)) ||
         (FontMaterialNumbers[i] == 13 && "KW".Contains(ModuleLetterText[i].text)) || (FontMaterialNumbers[i] == 17 && "?!".Contains(ModuleLetterText[i].text))) {
            goto Reshuffle;
         }
      }
      Debug.LogFormat("[Letter Grid #{0}] The encryptions in reading order are as follows:", moduleId);
      Debug.LogFormat("[Letter Grid #{0}] {1} | {2} | {3} | {4} | {5}", moduleId, EncryptionsName[FontMaterialNumbers[0]], EncryptionsName[FontMaterialNumbers[1]], EncryptionsName[FontMaterialNumbers[2]], EncryptionsName[FontMaterialNumbers[3]], EncryptionsName[FontMaterialNumbers[4]]);
      Debug.LogFormat("[Letter Grid #{0}] {1} | {2} | {3} | {4} | {5}", moduleId, EncryptionsName[FontMaterialNumbers[5]], EncryptionsName[FontMaterialNumbers[6]], EncryptionsName[FontMaterialNumbers[7]], EncryptionsName[FontMaterialNumbers[8]], EncryptionsName[FontMaterialNumbers[9]]);
      Debug.LogFormat("[Letter Grid #{0}] {1} | {2} | {3} | {4} | {5}", moduleId, EncryptionsName[FontMaterialNumbers[10]], EncryptionsName[FontMaterialNumbers[11]], EncryptionsName[FontMaterialNumbers[12]], EncryptionsName[FontMaterialNumbers[13]], EncryptionsName[FontMaterialNumbers[14]]);
      Debug.LogFormat("[Letter Grid #{0}] {1} | {2} | {3} | {4} | {5}", moduleId, EncryptionsName[FontMaterialNumbers[15]], EncryptionsName[FontMaterialNumbers[16]], EncryptionsName[FontMaterialNumbers[17]], EncryptionsName[FontMaterialNumbers[18]], EncryptionsName[FontMaterialNumbers[19]]);
      Debug.LogFormat("[Letter Grid #{0}] {1} | {2} | {3} | {4} | {5}", moduleId, EncryptionsName[FontMaterialNumbers[20]], EncryptionsName[FontMaterialNumbers[21]], EncryptionsName[FontMaterialNumbers[22]], EncryptionsName[FontMaterialNumbers[23]], EncryptionsName[FontMaterialNumbers[24]]);
      for (int i = 0; i < 25; i++) {
         if (FontMaterialNumbers[i] < 20) {
            ModuleLetterText[i].font = Fonts[FontMaterialNumbers[i]];
            ModuleLetterText[i].GetComponent<Renderer>().material = FontMats[FontMaterialNumbers[i]];
            switch (FontMaterialNumbers[i]) {
               case 0:
                  if (ModuleLetterText[i].text != "L") {
                     ModuleLetterText[i].fontSize = 100;
                  }
                  break;
               case 9: //lombax
                  ModuleLetterText[i].transform.localPosition -= new Vector3(0, 0, .001f);
                  break;
               case 10: //moon
                  ModuleLetterText[i].transform.localPosition += new Vector3(.003f, 0, 0);
                  break;
               case 11: //pigpen
                  ModuleLetterText[i].text = PigpenSwap[Alphabet.IndexOf(ModuleLetterText[i].text)];
                  break;
               case 12:
               case 13: //necor futhark
                  ModuleLetterText[i].text = ModuleLetterText[i].text.ToLower();
                  break;
               case 17: //sync
                  ModuleLetterText[i].text = Sync[Alphabet.IndexOf(ModuleLetterText[i].text)];
                  break;
               case 19: //zoni
                  ModuleLetterText[i].transform.localPosition -= new Vector3(0, 0, 0.003f);
                  break;
            }
         }
         else {
            switch (FontMaterialNumbers[i]) {
               case 20:
                  Morse.transform.localPosition += new Vector3(.2f * (i % 5), 0, -.2f * (i / 5));
                  StartCoroutine(MorseFlash(ModuleLetterText[i].text));
                  ModuleLetterText[i].text = "";
                  break;
               case 21:
                  Tap.transform.localPosition += new Vector3(.2f * (i % 5), 0, -.2f * (i / 5));
                  StartCoroutine(TapFlash(ModuleLetterText[i].text));
                  ModuleLetterText[i].text = "";
                  break;
               case 22:
                  More.transform.localPosition += new Vector3(.2f * (i % 5), 0, -.2f * (i / 5));
                  StartCoroutine(MoreFlash(ModuleLetterText[i].text));
                  ModuleLetterText[i].text = "";
                  break;
               case 23:
                  Binary.transform.localPosition += new Vector3(.2f * (i % 5), 0, -.2f * (i / 5));
                  ModuleLetterText[i].transform.localPosition += new Vector3(0, 0.01635f - 0.01566f, 0);
                  ModuleLetterText[i].font = Fonts[6];
                  ModuleLetterText[i].GetComponent<Renderer>().material = FontMats[6];
                  StartCoroutine(BinaryFlash(ModuleLetterText[i].text, ModuleLetterText[i]));
                  break;
               case 24:
                  Maritime.transform.localPosition += new Vector3(.2f * (i % 5), 0, -.2f * (i / 5));
                  Maritime.GetComponent<SpriteRenderer>().sprite = Flags[Alphabet.IndexOf(GridOfLetters[i])];
                  ModuleLetterText[i].text = "";
                  break;
            }
         }
      }
   }

   IEnumerator MorseFlash (string x) {
      string Letter = MorseLetters[Alphabet.IndexOf(x)];
      while (true) {
         for (int i = 0; i < Letter.Length; i++) {
            if (Letter[i] == '.') {
               Morse.GetComponent<MeshRenderer>().material = MorseMat;
               yield return new WaitForSecondsRealtime(.2f);
               Morse.GetComponent<MeshRenderer>().material = OffMat;
            }
            else {
               Morse.GetComponent<MeshRenderer>().material = MorseMat;
               yield return new WaitForSecondsRealtime(.6f);
               Morse.GetComponent<MeshRenderer>().material = OffMat;
            }
            yield return new WaitForSecondsRealtime(.2f);
         }
         yield return new WaitForSecondsRealtime(1f);
      }
   }

   IEnumerator TapFlash (string x) {
      while (true) {
         if (x != "K") {
            for (int i = 0; i < (Alphabet.IndexOf(x) / 5) + 1; i++) {
               Tap.GetComponent<MeshRenderer>().material = TapMat;
               yield return new WaitForSecondsRealtime(.2f);
               Tap.GetComponent<MeshRenderer>().material = OffMat;
               yield return new WaitForSecondsRealtime(.2f);
            }
            yield return new WaitForSecondsRealtime(.3f);
            for (int i = 0; i < (Alphabet.IndexOf(x) % 5) + 1; i++) {
               Tap.GetComponent<MeshRenderer>().material = TapMat;
               yield return new WaitForSecondsRealtime(.2f);
               Tap.GetComponent<MeshRenderer>().material = OffMat;
               yield return new WaitForSecondsRealtime(.2f);
            }
         }
         else {
            for (int i = 0; i < 6; i++) {
               Tap.GetComponent<MeshRenderer>().material = TapMat;
               yield return new WaitForSecondsRealtime(.2f);
               Tap.GetComponent<MeshRenderer>().material = OffMat;
            }
            yield return new WaitForSecondsRealtime(.3f);
            for (int i = 0; i < 6; i++) {
               Tap.GetComponent<MeshRenderer>().material = TapMat;
               yield return new WaitForSecondsRealtime(.2f);
               Tap.GetComponent<MeshRenderer>().material = OffMat;
            }
         }
         yield return new WaitForSecondsRealtime(1f);
      }
   }

   IEnumerator MoreFlash (string x) {
      string Letter = Morephabet[Alphabet.IndexOf(x)];
      while (true) {
         for (int i = 0; i < Letter.Length; i++) {
            if (Letter[i] == '.') {
               More.GetComponent<MeshRenderer>().material = MoreMat;
               yield return new WaitForSecondsRealtime(.6f);
               More.GetComponent<MeshRenderer>().material = OffMat;
            }
            else if (Letter[i] == ',') {
               More.GetComponent<MeshRenderer>().material = MoreMat;
               yield return new WaitForSecondsRealtime(.2f);
               More.GetComponent<MeshRenderer>().material = OffMat;
            }
            else if (Letter[i] == '=') {
               More.GetComponent<MeshRenderer>().material = MoreMat;
               yield return new WaitForSecondsRealtime(1f);
               More.GetComponent<MeshRenderer>().material = OffMat;
            }
            else {
               More.GetComponent<MeshRenderer>().material = MoreMat;
               yield return new WaitForSecondsRealtime(3f);
               More.GetComponent<MeshRenderer>().material = OffMat;
            }
            yield return new WaitForSecondsRealtime(1f);
         }
         yield return new WaitForSecondsRealtime(1f);
      }
   }

   IEnumerator BinaryFlash (string x, TextMesh Text) {
      while (true) {
         for (int i = 0; i < 5; i++) {
            if (BinaryRepresentations[Alphabet.IndexOf(x)][i]) {
               Binary.GetComponent<MeshRenderer>().material = BinaryMat;
            }
            else {
               Binary.GetComponent<MeshRenderer>().material = OffMat;
            }
            Text.text = (i + 1).ToString();
            yield return new WaitForSecondsRealtime(.2f);
            Text.text = "";
            Binary.GetComponent<MeshRenderer>().material = OffMat;
            yield return new WaitForSecondsRealtime(.2f);
         }
      }
   }

   void ArrowPress (KMSelectable Arrow) {
      Arrow.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Arrow.transform);
      if (moduleSolved) {
         Audio.PlaySoundAtTransform("correct", this.transform);
         return;
      }
      for (int i = 0; i < 10; i++) {
         if (Arrow == Arrows[i]) {
            switch (i) {
               case 0:
               case 1:
               case 2:
               case 3:
               case 4:
                  if (CheckIfWordIsPresent(true, i)) {
                     GetComponent<KMBombModule>().HandlePass();
                     Audio.PlaySoundAtTransform("correct", this.transform);
                     moduleSolved = true;
                  }
                  else {
                     GetComponent<KMBombModule>().HandleStrike();
                  }
                  break;
               case 5:
               case 6:
               case 7:
               case 8:
               case 9:
                  if (CheckIfWordIsPresent(false, (i - 5) * 5)) {
                     GetComponent<KMBombModule>().HandlePass();
                     Audio.PlaySoundAtTransform("correct", this.transform);
                     moduleSolved = true;
                  }
                  else {
                     GetComponent<KMBombModule>().HandleStrike();
                  }
                  break;
            }
         }
      }
   }

   bool CheckIfWordIsPresent (bool Col, int Start) {
      string TempCheck = "";
      if (Col) {
         for (int i = 0; i < 5; i++) {
            TempCheck += GridOfLetters[Start + i * 5].ToString();
         }
         for (int i = 0; i < 25; i++) {
            if (TempCheck == ValidWords[i]) {
               return true;
            }
         }
         TempCheck = "";
         for (int i = 4; i > -1; i--) {
            TempCheck += GridOfLetters[Start + i * 5].ToString();
         }
         for (int i = 0; i < 25; i++) {
            if (TempCheck == ValidWords[i]) {
               return true;
            }
         }
         return false;
      }
      else {
         for (int i = 0; i < 5; i++) {
            TempCheck += GridOfLetters[Start + i].ToString();
         }
         for (int i = 0; i < 25; i++) {
            if (TempCheck == ValidWords[i]) {
               return true;
            }
         }
         TempCheck = "";
         for (int i = 4; i > -1; i--) {
            TempCheck += GridOfLetters[Start + i].ToString();
         }
         for (int i = 0; i < 25; i++) {
            if (TempCheck == ValidWords[i]) {
               return true;
            }
         }
         return false;
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} row/column 1/2/3/4/5 to select that row or column.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      string[] Parameters = Command.Trim().ToUpper().Split(' ');
      if (Parameters.Length != 2) {
         yield return "sendtochaterror I don't understand!";
         yield break;
      }
      else if (!"ROW".Contains(Parameters[0]) && !"12345".Contains(Parameters[1]) && !"COLUMN".Contains(Parameters[0]) && Parameters[1].Length != 1) {
         yield return "sendtochaterror I don't understand!";
         yield break;
      }
      else {
         if (Parameters[0] == "ROW") {
            Arrows[int.Parse(Parameters[1]) + 4].OnInteract();
         }
         else {
            Arrows[int.Parse(Parameters[1]) - 1].OnInteract();
         }
      }
      yield return new WaitForSecondsRealtime(.1f);
   }

   IEnumerator TwitchHandleForcedSolve () {
      for (int i = 0; i < 10; i++) {
         switch (i) {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
               if (CheckIfWordIsPresent(true, i)) {
                  Arrows[i].OnInteract();
                  yield break;
               }
               break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
               if (CheckIfWordIsPresent(false, (i - 5) * 5)) {
                  Arrows[i].OnInteract();
                  yield break;
               }
               break;
         }
      }
   }
}