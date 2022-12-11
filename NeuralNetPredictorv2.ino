#include <ECE3.h>
#include <stdio.h>

uint16_t sensorValues[8];

int weightIndex = 0;
int layers[] = {8, 8, 4, 2, 1};

float weights1[64] = {-0.24699482,-0.23277318,-0.2233314,-0.058616146,0.06255431,0.12688133,0.008558387,0.3667457,-0.058871105,-0.008610962,-0.023531752,0.05788555,-0.15440184,-0.12598997,-0.1742946,-0.09219311,0.42985925,0.20070118,0.10440133,-0.03676703,0.09337459,0.049748547,-0.14776747,-0.2217157,-0.1663293,-0.062322356,-0.2221116,-0.22991812,-0.12167092,-0.1548494,0.19957909,-0.21282126,0.37517357,0.12876861,-0.101469144,0.091668755,-0.11311921,-0.10347268,-0.009541602,-0.12219662,0.25702214,-0.18323536,0.026749242,0.20660213,0.16607296,0.089133844,-0.1736087,0.2542313,-0.19416018,0.1021777,-0.19138478,-0.036935482,-0.05349968,-0.07669722,0.041646607,0.039572496,-0.043837942,-0.11307007,-0.17158516,-0.07433256,-0.050532088,0.05473177,0.21545827,0.3029379};
float weights2[32] = {-0.3273443,-0.20911357,0.4730244,-0.057344276,0.37848842,0.17732382,0.0093926275,-0.15571105,0.27186698,0.07338978,-0.35530227,0.12635079,-0.063777894,0.16298638,0.053236224,0.091359235,0.10121811,0.11764077,0.1641996,0.05330113,0.19147307,0.049246654,-0.12919922,-0.07375531,0.37642625,0.20508938,0.040092107,0.0815082,-0.051347002,0.26911244,-0.082823776,0.28541216};
float weights3[8] = {-0.20863196,-0.115010396,0.20263068,-0.086423844,-0.74871314,0.47187468,-0.17797549,0.42325103};
float weights4[2] = {-0.037091527,1.0279415};

float biases1[8] = {0.2266396,-0.035648026,0.19488177,-0.004714145,0.11936809,0.046222016,0.027682105,0.094741374};
float biases2[4] = {0.12331193,0.14046752,-0.042515263,0.12476696};
float biases3[2] = {0,0.40639848};
float bias4 = 0.06803763;

void setup()
{
  ECE3_Init();
  Serial.begin(9600); // set the data rate in bits per second for serial data transmission
  delay(2000);
}

float ReLU(float x)
{
    return (x > 0) ? x : 0;
}

void loop()
{
  // read raw sensor values
  
  float neurons0[8] = {0,0,0,0,0,0,0,0};
  float neurons1[8] = {0,0,0,0,0,0,0,0};
  float neurons2[4] = {0,0,0,0};
  float neurons3[2] = {0,0};
  float output = 0;
  ECE3_read_IR(sensorValues);
  float min1 = 9999.0;
  float max1 = -1.0;
  for (unsigned char i = 0; i < 8; i++)
  {
    //Serial.print(sensorValues[i]);
    //Serial.print('\t'); // tab to format the raw data into columns in the Serial monitor
    neurons0[i] = sensorValues[i];
  }
  //Serial.println();
  
  for (int i = 0; i < 8; i++)
  {
    if (neurons0[i] < min1)
    {
      min1 = sensorValues[i];
    }
    if (neurons0[i] > max1)
    {
      max1 = sensorValues[i];
    }
  }
  max1 = max1 - min1;
  for (int i = 0; i < sizeof(neurons0)/sizeof(float); i++)
  {
    neurons0[i] = (neurons0[i] - min1)/max1;
    //Serial.print(neurons0[i]);
    //Serial.print('\t'); // tab to format the raw data into columns in the Serial monitor
  }
  weightIndex = 0;
  for (int i = 0; i < sizeof(neurons1)/sizeof(float); i++)
  {
    for (int j = 0; j < sizeof(neurons0)/sizeof(float); j++)
    {
      neurons1[i] += neurons0[j]*weights1[weightIndex];
      weightIndex++;
    }
    neurons1[i] = ReLU(neurons1[i] + biases1[i]);
  }
  weightIndex = 0;
  for (int i = 0; i < sizeof(neurons2)/sizeof(float); i++)
  {
    for (int j = 0; j < sizeof(neurons1)/sizeof(float); j++)
    {
      neurons2[i] += neurons1[j]*weights2[weightIndex];
      weightIndex++;
    }
    neurons2[i] = ReLU(neurons2[i] + biases2[i]);
  }
  weightIndex = 0;
  for (int i = 0; i < sizeof(neurons3)/sizeof(float); i++)
  {
    for (int j = 0; j < sizeof(neurons2)/sizeof(float); j++)
    {
      neurons3[i] += neurons2[j]*weights3[weightIndex];
      weightIndex++;
    }
    neurons3[i] = ReLU(neurons3[i] + biases3[i]);
  }

  output = ReLU(neurons3[0]*weights4[0]+neurons3[1]+weights4[1]+bias4);
  //Serial.println(sizeof(neurons3)/sizeof(float));
  Serial.print("Predicted distance is: ");
  //Serial.println(output);
  Serial.println(int((output-0.5)*80));

  delay(50);
}
