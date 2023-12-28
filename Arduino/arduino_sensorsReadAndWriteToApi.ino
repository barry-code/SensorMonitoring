#include <WiFi.h>
#include <SimpleDHT.h>
#include <HTTPClient.h>

//This is the script running on an arduino, which is connected to multiple sensors, and reads the values of sensors and writes them to SensorMonitoring API.

// Wifi Stuff
const char* ssid = "WifiNameGoesHere";
const char* password = "WifPasswordGoesHere";
bool wifiConnected = false;
int disconnectEvents = 0;

//Sensor IO Stuff (currently unused sensors commented out)
int pinDHT22_upstairs = 23;
int pinDHT22_attic1 = 18;
//int pinDHT22_downstairs = 5;
//int pinDHT22_attic2 = 19;
SimpleDHT22 dht22_upstairs(pinDHT22_upstairs);
SimpleDHT22 dht22_attic1(pinDHT22_attic1);
//SimpleDHT22 dht22_downstairs(pinDHT22_downstairs);
//SimpleDHT22 dht22_attic2(pinDHT22_attic2);
bool upstairsEnabled = true;
bool attic1Enabled = true;
//bool downstairsEnabled = false;
//bool attic2Enabled = false;

//each of the 4 DHT sensors(upstairs, downstairs, attic1, attic2), has two sensors (1XTemp & 1XHumidity) = 8 total sensors
const int totalSensorsCount = 8;

// SensorIDs in sqlite database needed for writing to api
const int downstairsTempDbId = 3;
const int downstairsHumidityDbId = 4;
const int upstairsTempDbId = 5;
const int upstairsHumidityDbId = 6;
const int attic1TempDbId = 7;
const int attic1HumidityDbId = 8;
const int attic2TempDbId = 9;
const int attic2HumidityDbId = 10;

//readings array references
const int downstairsTempArrayId = 0;
const int downstairsHumidityArrayId = 1;
const int upstairsTempArrayId = 2;
const int upstairsHumidityArrayId = 3;
const int attic1TempArrayId = 4;
const int attic1HumidityArrayId = 5;
const int attic2TempArrayId = 6;
const int attic2HumidityArrayId = 7;

class SensorReading{
  private:
  bool hasNewValue;
  int sensorId;
  float readingValue;

  public:
  SensorReading(int sId){
    sensorId = sId;
    hasNewValue = false;
  }
  void SetValue(float rValue){
    hasNewValue = true;
    readingValue = rValue;
  }
  void Reset(){
    hasNewValue = false;
  }
  int GetSensorId(){
    return sensorId;
  }
  float GetReading(){
    return readingValue;
  }
  bool HasValue(){
    return hasNewValue;
  }
};

SensorReading readings[] = {
  SensorReading(downstairsTempDbId), 
  SensorReading(downstairsHumidityDbId),
  SensorReading(upstairsTempDbId),
  SensorReading(upstairsHumidityDbId),
  SensorReading(attic1TempDbId),
  SensorReading(attic1HumidityDbId),
  SensorReading(attic2TempDbId),
  SensorReading(attic2HumidityDbId)};

void wifi_connected(WiFiEvent_t wifi_event, WiFiEventInfo_t wifi_info){
    Serial.println("\n[+] Connected to the WiFi network");    
}

void wifi_disconnected(WiFiEvent_t wifi_event, WiFiEventInfo_t wifi_info){
    wifiConnected = false;
    disconnectEvents++;
    Serial.println("\n[-] Disconnected from the WiFi network!!!");
    Serial.print((String)"Attempting reconnect in " + 1000 * disconnectEvents + "ms");
    delay(1000 * disconnectEvents);

    if(disconnectEvents > 3000){
      Serial.println("\nRESTARTING SYSTEM...");
      ESP.restart();
      delay(30000);
      Serial.println("\nFINISHED RESTART");
    }
    else if(disconnectEvents > 0){
      Serial.println("\nreconnecting....");
        WiFi.begin(ssid, password);
    }
}

void wifi_got_ip(WiFiEvent_t wifi_event, WiFiEventInfo_t wifi_info){
    Serial.print("[+] Local ESP32 IP: ");
    Serial.println(WiFi.localIP());
    Serial.println((String)"[+] RSSI : " + WiFi.RSSI() + " dB");
    wifiConnected = true;
    disconnectEvents = 0;
}

void wifi_lost_ip(WiFiEvent_t wifi_event, WiFiEventInfo_t wifi_info){
    Serial.println("\n[-] LOST IP ADDRESS");    
}

void SetSensorValues(float temp, float hum, int tempArrayId, int humArrayId){          
    Serial.print(temp);
    Serial.print(" *C, ");
    Serial.print(hum);
    Serial.println(" RH%");
    readings[tempArrayId].SetValue(temp);
    readings[humArrayId].SetValue(hum);
}

void ResetSensors(){
  for(int i = 0; i < totalSensorsCount; i++){
    readings[i].Reset();
  }
}

void WriteSensorReadingsToApi(){
  bool sensorDataAvailable = false;
  String fullJsonData = "[";
  for(int i = 0; i < totalSensorsCount; i++){
    if(readings[i].HasValue()){
      String sensorJson = "\"sensorId\":";
      String readingJson = "\"reading\":";

      if(sensorDataAvailable){
        fullJsonData += ",";
      }
      sensorJson += String(readings[i].GetSensorId());
      readingJson += String(readings[i].GetReading());

      fullJsonData += "{" + sensorJson + "," + readingJson + "}";

      sensorDataAvailable = true;
    }
  }
  fullJsonData += "]";

  if(!sensorDataAvailable){
    Serial.println("no sensor data to write to api");
    return;
  }
  else{
    Serial.print("sending json data:");
    Serial.println(fullJsonData);
  }

  HTTPClient http;
  
  http.begin("http://SensorMonitoringApiIpAddress:PortNumber/Sensor/AddSensorReadings");
  http.addHeader("Content-Type", "application/json");
  int httpCode = http.POST(fullJsonData);
  
  if(httpCode > 0) {
    // HTTP header has been send and Server response header has been handled
    Serial.printf("[HTTP] POST... code: %d\n", httpCode);

    // file found at server
    if(httpCode == HTTP_CODE_OK) {
        String payload = http.getString();
        Serial.println(payload);
    }
  } 
  else {
    Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
  }

  http.end();
}

void setup(){
    Serial.begin(9600); //temp sensors
    Serial.begin(115200); //wifi

    WiFi.mode(WIFI_STA); //Optional
    
    WiFi.onEvent(wifi_connected, ARDUINO_EVENT_WIFI_STA_CONNECTED);
    WiFi.onEvent(wifi_got_ip, ARDUINO_EVENT_WIFI_STA_GOT_IP);
    WiFi.onEvent(wifi_lost_ip, ARDUINO_EVENT_WIFI_STA_LOST_IP);
    WiFi.onEvent(wifi_disconnected, ARDUINO_EVENT_WIFI_STA_DISCONNECTED);

    delay(2000);

    WiFi.begin(ssid, password);
}

void loop(){

    float temperature1 = 0;
    float humidity1 = 0;
    float temperature2 = 0;
    float humidity2 = 0;
    float temperature3 = 0;
    float humidity3 = 0;
    float temperature4 = 0;
    float humidity4 = 0;

    int err = SimpleDHTErrSuccess;
    
    if(wifiConnected)
    {      
      Serial.println("\nsensor loop running...");

      ResetSensors();

      //SENSOR 1 (UPSTAIRS)
      if (upstairsEnabled)
      {
        if ((err = dht22_upstairs.read2(&temperature1, &humidity1, NULL)) != SimpleDHTErrSuccess) 
        {
          Serial.print("Read Sensor (Upstairs) failed, err=");
          Serial.println(err);
        }
        else
        {
          Serial.print("Sensor (Upstairs) OK: ");        
          SetSensorValues((float)temperature1, (float)humidity1, upstairsTempArrayId, upstairsHumidityArrayId);
        }
        delay(1000);
      }

      //SENSOR 2 (ATTIC1)
      if(attic1Enabled)
      {
        if ((err = dht22_attic1.read2(&temperature2, &humidity2, NULL)) != SimpleDHTErrSuccess) 
        {
          Serial.print("Read Sensor (Attic 1) failed, err=");
          Serial.println(err);
        }
        else
        {
          Serial.print("Sensor (Attic 1) OK: ");
          SetSensorValues((float)temperature2, (float)humidity2, attic1TempArrayId, attic1HumidityArrayId);
        }
      }

      WriteSensorReadingsToApi();      

      delay(1800000); //sleep 30mins
      //delay(60000); //sleep 1min
    }
    else
    {
      Serial.println("\nwaiting for wifi...");
      delay(10000); //sleep 10seconds
    }    
}