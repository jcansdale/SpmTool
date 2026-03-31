<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>
  <xsl:param name="modelName" />

  <xsl:template match="/">
    <xsl:copy>
      <xsl:apply-templates select="SPM" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="SPM">
    <xsl:copy>
      <xsl:apply-templates mode="top" select="Spektrum|Acro|Servo|DR_Expo|P-Mix|FMode|SoftSw|Special|TrimID|Trim|ThroCut|ThroCurve|RAE-Mix|EF-Mix|AR-Mix|FlapSystem|Differential|Config|Timer|Warning|Telemetry|Trainer|Heli|PitchCurve|RevoCurve|Gyro|Governor|C-Mix|S-Mix|SwashPlate|Sail" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Spektrum">
    <xsl:copy>
      <xsl:apply-templates select="Generator|PosIndex|Type|curveIndex|Name|PosMaxSail" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Spektrum/Generator/text()">DX8</xsl:template>

  <xsl:template match="Spektrum/Name/text()">
    <xsl:value-of select="$modelName" />
  </xsl:template>

  <xsl:template mode="top" match="Telemetry">
    <xsl:copy>
      <!-- Too many changes in DX9 FlightLog|Module to support -->
      <!-- <xsl:apply-templates select="lcdMode|Screens|StartID|Event|Thresh|oneTime|FlightLog|Module" /> -->
      <xsl:apply-templates select="lcdMode|Screens|StartID|Event|Thresh|oneTime" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Trainer">
    <xsl:copy>
      <xsl:apply-templates select="Type" />
      <xsl:if test="mixRatio">
        <Active Type="Array">
          <xsl:for-each select="mixRatio/Element">
            <xsl:choose>
              <xsl:when test="text()=0">
                <Element>Disabled</Element>
              </xsl:when>
              <xsl:when test="text()=100">
                <Element>Enabled</Element>
              </xsl:when>
              <xsl:otherwise>
                <Element>Enabled</Element>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:for-each>
        </Active>
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Telemetry/FlightLog/sdEnabled"></xsl:template>

  <!-- Sailplane -->

  <xsl:template mode="top" match="Sail">
    <xsl:copy>
      <xsl:apply-templates select="Wing|Tail|Motor" />
    </xsl:copy>
  </xsl:template>

  <!-- Helicopter -->
  
  <xsl:template mode="top" match="Heli">
    <xsl:copy>
      <xsl:apply-templates select="Swash" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Gyro">
    <xsl:copy>
      <xsl:apply-templates select="sourceID|conditionID|trimID|fpct|tailHold|outChan" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Governor">
    <xsl:copy>
      <xsl:apply-templates select="sourceID|conditionID|trimID|fpct|outChan" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="C-Mix">
    <xsl:copy>
      <xsl:apply-templates select="rateHighAilThr|rateLowAilThr|rateHighEleThr|rateLowEleThr|rateHighRudThr|rateLowRudThr|activePositions" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="S-Mix">
    <xsl:copy>
      <xsl:apply-templates select="rateHighAilEle|rateLowAilEle|rateHighEleAil|rateLowEleAil|activePositions" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="SwashPlate">
    <xsl:copy>
      <xsl:apply-templates select="rateAileron|rateElevator|ratePitch|E-Ring|Expo|rateExpo" />
    </xsl:copy>
  </xsl:template>

  <!-- Airplane -->
  
  <xsl:template mode="top" match="Acro">
    <xsl:copy>
      <xsl:apply-templates select="Wing|Tail" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Servo[Index&lt;8]">
    <xsl:copy>
      <xsl:apply-templates select="Index|sourceID|speed|direction|subTrim|travelLow|travelHigh|name" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="DR_Expo">
    <xsl:copy>
      <xsl:apply-templates select="Index|analogID|conditionID|activePositions|drHigh|drLow|expoHigh|expoLow" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="DR_Expo/drHigh/Element[position()>5]"></xsl:template>
  <xsl:template match="DR_Expo/drLow/Element[position()>5]"></xsl:template>
  <xsl:template match="DR_Expo/expoHigh/Element[position()>5]"></xsl:template>
  <xsl:template match="DR_Expo/expoLow/Element[position()>5]"></xsl:template>

  <xsl:template mode="top" match="P-Mix[Index&lt;6]">
    <xsl:copy>
      <xsl:apply-templates select="Index|analogID|conditionID|trimID|activePositions|outChan|Curvedata" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="FMode">
    <xsl:copy>
      <xsl:choose>
        <xsl:when test="/SPM/Spektrum/Type/text()='Heli'">
          <xsl:apply-templates select="switch_a" />
          <switch_b><xsl:apply-templates select="switch_c/text()" /></switch_b>
          <switch_c>0</switch_c>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="switch_a|switch_b|switch_c" />
        </xsl:otherwise>
      </xsl:choose>
      <size>9</size>
      <xsl:if test="fmtable">
        <data Type="Array">
          <xsl:choose>
            <xsl:when test="/SPM/Spektrum/Type/text()='Heli'">
                <xsl:for-each select="fmtable/Element">
                  <xsl:choose>
                    <xsl:when test="(position()-1) mod 6 = 0">
                      <xsl:choose>
                        <xsl:when test="../../activePositions/text()='%0001' or ../../activePositions/text()='%0003' or ../../activePositions/text()='%0005' or ../../activePositions/text()='%0007'"><Element>0</Element></xsl:when>
                        <xsl:otherwise><xsl:copy-of select="." /></xsl:otherwise>
                      </xsl:choose>
                      <xsl:choose>
                        <xsl:when test="../../activePositions/text()='%0002' or ../../activePositions/text()='%0003' or ../../activePositions/text()='%0006' or ../../activePositions/text()='%0007'"><Element>0</Element></xsl:when>
                        <xsl:otherwise><xsl:copy-of select="." /></xsl:otherwise>
                      </xsl:choose>
                      <xsl:choose>
                        <xsl:when test="../../activePositions/text()='%0004' or ../../activePositions/text()='%0005' or ../../activePositions/text()='%0006' or ../../activePositions/text()='%0007'"><Element>0</Element></xsl:when>
                        <xsl:otherwise><xsl:copy-of select="." /></xsl:otherwise>
                      </xsl:choose>
                    </xsl:when>
                  </xsl:choose>
                </xsl:for-each>
            </xsl:when>
            <xsl:otherwise>
                <xsl:for-each select="fmtable/Element">
                  <xsl:if test="(position()-1) mod 6 &lt; 3">
                    <xsl:copy-of select="." />
                  </xsl:if>
                </xsl:for-each>
            </xsl:otherwise>
          </xsl:choose>
        </data>
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="SoftSw">
    <xsl:copy>
      <xsl:apply-templates select="Index|sourceID" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Special">
    <xsl:copy>
      <xsl:apply-templates select="Index|sourceID" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="TrimID[Index&lt;8]">
    <xsl:copy>
      <xsl:apply-templates select="Index|sourceID" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Trim">
    <xsl:copy>
      <xsl:apply-templates select="Index|Pos0|Pos1|Pos2|trimClicks|maxTrimClicks|trimStepSize|trimRepeat|trimNextRepeat" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="ThroCut">
    <xsl:copy>
      <xsl:apply-templates select="conditionID|percent|rampSpeed|activePositions" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Trim/trimClicks/Element[position()>5]"></xsl:template>

  <xsl:template mode="top" match="PitchCurve">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|activeMask|delay|assignedCurve|Curvedata" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="PitchCurve/assignedCurve/Element[position()>5]"></xsl:template>

  <xsl:template match="PitchCurve/Curvedata/curved"></xsl:template>

  <xsl:template mode="top" match="ThroCurve">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|activeMask|delay|assignedCurve|Curvedata" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="ThroCurve/assignedCurve/Element[position()>5]"></xsl:template>

  <xsl:template match="ThroCurve/Curvedata/curved"></xsl:template>

  <xsl:template mode="top" match="RevoCurve">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|activeMask|delay|assignedCurve|Curvedata" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="RevoCurve/assignedCurve/Element[position()>5]"></xsl:template>

  <xsl:template match="RevoCurve/Curvedata/curved"></xsl:template>

  <xsl:template mode="top" match="RAE-Mix">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|percentAileron|percentElevator|activePositions" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="EF-Mix">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|activePositions|outChan|Curvedata|efItem" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="EF-Mix/efItem">
    <efItem Type="Object">
      <xsl:copy-of select="Index" />
      <xsl:copy-of select="offset" />
      <xsl:element name="flapUp">
        <xsl:value-of select="flapLeft/text()" />
      </xsl:element>
      <xsl:element name="flapDown">
        <xsl:value-of select="flapRight/text()" />
      </xsl:element>
      <xsl:element name="flonUp">
        <xsl:value-of select="flonLeft/text()" />
      </xsl:element>
      <xsl:element name="flonDown">
        <xsl:value-of select="flonRight/text()" />
      </xsl:element>
    </efItem>
  </xsl:template>

  <xsl:template mode="top" match="AR-Mix">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|activePositions|outChan|Curvedata" />
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="AR-Mix/Curvedata/Y/Element/text()">
    <xsl:value-of select="-." />
  </xsl:template>

  <xsl:template mode="top" match="FlapSystem">
    <xsl:copy>
      <xsl:apply-templates select="analogID|conditionID|trimID|speed|flapTarget|elevatorTarget|Curvedata" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Differential">
    <xsl:copy>
      <xsl:apply-templates select="conditionID|rate" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="top" match="Config">
    <xsl:copy>
      <xsl:apply-templates select="TrimType|trimMode" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Config/TrimType/text()">
    <xsl:choose>
      <xsl:when test=".='%00000000'">Common</xsl:when> <!-- No FMode trims -->
      <xsl:when test=".='%0000003F'">FMode</xsl:when>  <!-- This is exact eqivalent -->
      <xsl:otherwise>FMode</xsl:otherwise>             <!-- Default to FMode? -->
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="top" match="Timer[Index&lt;1]">
    <xsl:copy>
      <xsl:apply-templates select="Index|Mode|Minutes|Seconds|oneTime|StartID|Event|Thresh|audioX|vibeX|Audio|Vibrate" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Timer/audioX">
    <Audio>
      <xsl:choose>
        <xsl:when test="text()='%00F4'">Enabled</xsl:when>
        <xsl:when test="text()='%0080'">Disabled</xsl:when>
        <xsl:when test="text()='%0000'">Disabled</xsl:when>
        <xsl:otherwise>Enabled</xsl:otherwise>
      </xsl:choose>
    </Audio>
  </xsl:template>

  <xsl:template match="Timer/vibeX">
    <Vibrate>
      <xsl:choose>
        <xsl:when test="text()='%0020'">Enabled</xsl:when>
        <xsl:when test="text()='%0000'">Disabled</xsl:when>
        <xsl:otherwise>Enabled</xsl:otherwise>
      </xsl:choose>
    </Vibrate>
  </xsl:template>

  <xsl:template mode="top" match="Warning">
    <xsl:copy>
      <xsl:choose>
        <xsl:when test="/SPM/Spektrum/Type/text()='Heli'">
          <xsl:apply-templates select="Vibrate|Throttle|Thresh|Gyro" />
          <FltMode>
            <xsl:choose>
              <xsl:when test="FltMode/text()='%0000' and (Hold/text()='%0000' or Hold/text()='%0001')">%0000</xsl:when>
              <xsl:when test="FltMode/text()='%0004' and (Hold/text()='%0000' or Hold/text()='%0001')">%0020</xsl:when>
              <xsl:when test="FltMode/text()='%0008' and (Hold/text()='%0000' or Hold/text()='%0001')">%0040</xsl:when>
              <xsl:when test="FltMode/text()='%000C' and (Hold/text()='%0000' or Hold/text()='%0001')">%0060</xsl:when>
              <xsl:when test="FltMode/text()='%0000' and Hold/text()='%0002'">%0080</xsl:when>
              <xsl:when test="FltMode/text()='%0004' and Hold/text()='%0002'">%00A0</xsl:when>
              <xsl:when test="FltMode/text()='%0008' and Hold/text()='%0002'">%00C0</xsl:when>
              <xsl:when test="FltMode/text()='%000C' and Hold/text()='%0002'">%00E0</xsl:when>
              <xsl:when test="FltMode/text()='%0010' and (Hold/text()='%0000' or Hold/text()='%0001')">%0000</xsl:when>
              <xsl:when test="FltMode/text()='%0014' and (Hold/text()='%0000' or Hold/text()='%0001')">%0020</xsl:when>
              <xsl:when test="FltMode/text()='%0018' and (Hold/text()='%0000' or Hold/text()='%0001')">%0040</xsl:when>
              <xsl:when test="FltMode/text()='%001C' and (Hold/text()='%0000' or Hold/text()='%0001')">%0060</xsl:when>
              <xsl:when test="FltMode/text()='%0010' and Hold/text()='%0002'">%0080</xsl:when>
              <xsl:when test="FltMode/text()='%0014' and Hold/text()='%0002'">%00A0</xsl:when>
              <xsl:when test="FltMode/text()='%0018' and Hold/text()='%0002'">%00C0</xsl:when>
              <xsl:when test="FltMode/text()='%001C' and Hold/text()='%0002'">%00E0</xsl:when>
              <xsl:otherwise>___UNKNOWN___ <xsl:value-of select ="text()" /></xsl:otherwise>
            </xsl:choose>
          </FltMode>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="Vibrate|Throttle|Thresh|Thresh|Gear" />
          <FltMode>
            <xsl:choose>
              <xsl:when test="FltMode/text()='%0000'">%0000</xsl:when>
              <xsl:when test="FltMode/text()='%0004'">%0020</xsl:when>
              <xsl:when test="FltMode/text()='%0008'">%0040</xsl:when>
              <xsl:when test="FltMode/text()='%000C'">%0060</xsl:when>
              <xsl:when test="FltMode/text()='%0010'">%0000</xsl:when>
              <xsl:when test="FltMode/text()='%0014'">%0020</xsl:when>
              <xsl:when test="FltMode/text()='%0018'">%0040</xsl:when>
              <xsl:when test="FltMode/text()='%001C'">%0060</xsl:when>
              <xsl:otherwise>___UNKNOWN___ <xsl:value-of select ="text()" /></xsl:otherwise>
            </xsl:choose>
          </FltMode>
          <xsl:apply-templates select="Flaps" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Warning/Flaps/text()">
    <xsl:choose>
      <xsl:when test=".='%0000'">%0000</xsl:when>
      <xsl:when test=".='%0002'">%0001</xsl:when>
      <xsl:when test=".='%0004'">%0002</xsl:when>
      <xsl:when test=".='%0006'">%0003</xsl:when>
      <xsl:when test=".='%0005'">%0004</xsl:when>
      <xsl:otherwise>
        ___UNKNOWN___ <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- ID mapping: DX9 value → DX8 value. See DX8toDX9.xslt for the reverse mapping. -->
  <xsl:template match="sourceID/text()|analogID/text()|conditionID/text()|trimID/text()|StartID/text()|outChan/text()|switch_a/text()|switch_b/text()|switch_c/text()">
    <xsl:choose>
      <!-- Inhibit -->
      <xsl:when test=".='0'">0</xsl:when>
      <!-- SoftSw/sourceID -->
      <xsl:when test=".='1'">1</xsl:when>    <!-- THR Servo Out -->
      <xsl:when test=".='5'">5</xsl:when>    <!-- GER Servo Out -->
      <!-- P-Mix/analogID -->
      <xsl:when test=".='6'">6</xsl:when>    <!-- AX1 -->
      <xsl:when test=".='7'">7</xsl:when>    <!-- AX2 -->
      <xsl:when test=".='8'">8</xsl:when>    <!-- AX3 -->
      <xsl:when test=".='9' and (/SPM/Acro/Tail/text()='Dual_Ele' or /SPM/Acro/Tail/text()='Dual_Rud_Ele')">7</xsl:when> <!-- AX4->AX2 -->
      <xsl:when test=".='9'">9</xsl:when>    <!-- AX4->AX4 -->
      <xsl:when test=".='64'">16</xsl:when>  <!-- Thr. Stick -->
      <xsl:when test=".='78'">32</xsl:when>  <!-- FLP -->
      <xsl:when test=".='79'">33</xsl:when>  <!-- GER -->
      <xsl:when test=".='128'">96</xsl:when> <!-- AIL -->
      <xsl:when test=".='129'">97</xsl:when> <!-- ELE -->
      <xsl:when test=".='130'">98</xsl:when> <!-- RUD -->
      <!-- DR_Expo/analogID -->
      <xsl:when test=".='65'">17</xsl:when> <!-- Ail. Stick -->
      <xsl:when test=".='66'">18</xsl:when> <!-- Ele. Stick -->
      <xsl:when test=".='67'">19</xsl:when> <!-- Rud. Stick -->
      <!-- Servo/sourceID -->
      <xsl:when test=".='69'">21</xsl:when>   <!-- R Knob  -->
      <xsl:when test=".='82'">40</xsl:when>   <!-- Gear->Switch A -->
      <xsl:when test=".='83'">41</xsl:when>   <!-- F Mode->Switch B -->
      <xsl:when test=".='85'">43</xsl:when>   <!-- Flap->Switch D -->
      <xsl:when test=".='86'">44</xsl:when>   <!-- AUX 2->Switch E -->
      <xsl:when test=".='89'">47</xsl:when>   <!-- Mix/Hold->Switch H -->
      <xsl:when test=".='92'">50</xsl:when>   <!-- Trainer->Switch I -->
      <xsl:when test=".='32'">192</xsl:when>  <!-- THR -->
      <xsl:when test=".='33'">193</xsl:when>  <!-- AIL -->
      <xsl:when test=".='34'">194</xsl:when>  <!-- ELE -->
      <xsl:when test=".='35'">195</xsl:when>  <!-- RUD -->
      <xsl:when test=".='39'">199</xsl:when>  <!-- AX3/FLP/LFL/GER -->
      <xsl:when test=".='52'">200</xsl:when>  <!-- FLP/LFL -->
      <xsl:when test=".='200'">244</xsl:when> <!-- Gyro -->
      <xsl:when test=".='201'">245</xsl:when> <!-- Governor -->
      <!-- DR_Expo/conditionID: 0, 42, 43, 45, 46, 63, 127 -->
      <xsl:when test=".='84'">42</xsl:when>   <!-- Elev D/R->Switch C -->
      <xsl:when test=".='87'">45</xsl:when>   <!-- Ail D/R->Switch F -->
      <xsl:when test=".='88'">46</xsl:when>   <!-- Rud D/R->Switch G -->
      <xsl:when test=".='107'">63</xsl:when>  <!-- On -->
      <xsl:when test=".='145'">127</xsl:when> <!-- Flight Mode -->
      <!-- Timer/StartID -->
      <xsl:when test=".='112'">68</xsl:when>  <!-- LTrimD -->
      <xsl:when test=".='113'">69</xsl:when>  <!-- RTrimD -->
      <!-- P-Mix/outChan -->
      <xsl:when test=".='36'">196</xsl:when>  <!-- GER -->
      <xsl:when test=".='37'">197</xsl:when>  <!-- AX1 -->
      <xsl:when test=".='38'">198</xsl:when>  <!-- AX2 -->
      <xsl:when test=".='40' and (/SPM/Acro/Tail/text()='Dual_Ele' or /SPM/Acro/Tail/text()='Dual_Rud_Ele')">198</xsl:when>  <!-- LEL -->
      <xsl:when test=".='40'">198</xsl:when>  <!-- AX4 -->
      <!-- P-Mix/trimID -->
      <xsl:when test=".='108'">64</xsl:when>  <!-- THR Trim -->
      <xsl:when test=".='109'">65</xsl:when>  <!-- AIL Trim -->
      <xsl:when test=".='110'">66</xsl:when>  <!-- ELE Trim -->
      <xsl:when test=".='111'">67</xsl:when>  <!-- RUD Trim -->
      <!-- UNKNOWN -->
      <xsl:otherwise>UNKNOWN</xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
