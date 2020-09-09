#include <FastLED.h>

#define LED_PIN     7
#define NUM_LEDS    102 //88 //51 //33

CRGB leds[NUM_LEDS];

int incomingByte = 0;
int ledNumber = 0;
String inString = "";
char inChar;
short charCounter = 0;

void setup() {
  FastLED.addLeds<WS2812, LED_PIN, GRB>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
  Serial.begin(250000);
  FastLED.setBrightness(70);
}

void loop() {
  if (Serial.available() > 0) {
    incomingByte = Serial.read(); // read the incoming byte:  
    if(incomingByte != 10)
      inString += (char)incomingByte; 
    charCounter++;
  }
  if(incomingByte == 10)
  {
    for(int i=0; i<charCounter/9; i++)
    {
      int r = inString.substring(0+9*i, 3+9*i).toInt();
      int g = inString.substring(3+9*i, 6+9*i).toInt();
      int b = inString.substring(6+9*i, 9+9*i).toInt();
      leds[i] = CRGB(r, g, b);
    }
    FastLED.show();
    inString = "";
    charCounter = 0;
    incomingByte = 0;
    Serial.print("Git");
    Serial.flush();
  }
}
