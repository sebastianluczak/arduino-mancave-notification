// include the library code:
#include <LiquidCrystal.h>

// initialize the library with the numbers of the interface pins
LiquidCrystal lcd(8, 9, 4, 5, 6, 7);

const int NOTIFICATION_LED_PIN = 44;
int ANALOG_PIN = A0; // used for debugging script from user input

bool shouldBlink = false;
int currentLEDValue = 0;
int initialLEDValue = 0;
int stepLEDValue = 1;
int maxLEDValue = 500;
float sinVal;
int ledVal;

int val = 0;  // variable to store the value read
// Used to hold notification type
String notificationType = "";
// Used to hold notification message
String message = "";

String inputString = "";
bool stringComplete = false;

void setup() {
  Serial.begin(9600);
  pinMode(NOTIFICATION_LED_PIN, OUTPUT);
  analogWrite(NOTIFICATION_LED_PIN, 0);
  message.reserve(256);
  lcd.begin(16, 2);
  lcd.setCursor(0, 0);
  lcd.clear();
}

void loop() {
  val = analogRead(ANALOG_PIN);
  if (stringComplete) {
    lcd.setCursor(0, 0);
    lcd.clear();
    shouldBlink = true;
    lcd.print(inputString);

        // clear the string:
    inputString = "";
    stringComplete = false;
  }

  blinkLed();
}

void blinkLed()
{
  if (shouldBlink) {
    shouldBlink = false;
    for (int x = 0; x < 20; x++) {
      sinVal = sin(x/9);
      ledVal = int(sinVal*255); // Why it is 255?
      analogWrite(NOTIFICATION_LED_PIN, ledVal);
      delay(25);
    }
    analogWrite(NOTIFICATION_LED_PIN, 0);
  }
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    } else {
      inputString += inChar;
    }
  }
}