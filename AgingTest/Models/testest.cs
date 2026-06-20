//using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

//const int ACS_PIN = 4;

//const float ADC_REF = 3.3;
//const int ADC_RES = 4095;

//float zeroCurrentVoltage = 0;
//float sensitivity = 0.040;

//void setup()
//{
//    Serial.begin(115200);
//}

//void loop()
//{
//    long total = 0;

//    for (int i = 0; i < 100; i++)
//    {
//        total += analogRead(ACS_PIN);
//        delay(2);
//    }

//    float adc = total / 100.0;

//    float voltage =
//        adc * ADC_REF / ADC_RES;

//    float current =
//        (voltage - zeroCurrentVoltage)
//        / sensitivity;

//    Serial.print("Voltage : ");
//    Serial.print(voltage, 3);

//    Serial.print(" V");

//    Serial.print(" | Current : ");
//    Serial.print(current, 2);

//    Serial.println(" A");

//    delay(500);
//}