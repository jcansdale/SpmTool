<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:spm="http://mutantdesign.co.uk/spm"
  exclude-result-prefixes="msxsl spm">

<xsl:output method="xml" indent="yes" />
  <xsl:param name="modelName" />
  <xsl:param name="generator" />
  <xsl:param name="masterVolume" />
  
  <xsl:template match="/">
    <xsl:apply-templates select="SPM" />
  </xsl:template>

  <xsl:template match="SPM">
    <SPM>
      <xsl:apply-templates mode="top" select="Spektrum|Acro|Trim|Servo|DR_Expo|ThroCut|P-Mix|Timer|FMode|EF-Mix|AR-Mix|FlapSystem|Differential|ThroCurve|Special|SoftSw|TrimID|Telemetry|Trainer|Heli|PitchCurve|RevoCurve|Gyro|Governor|RAE-Mix|C-Mix|S-Mix|SwashPlate|Warning|Config|Sail|CamberPreset|CamberMix|FlpEleMix|AR-Mix-S|AF-Mix-S" />

      <xsl:if test="(/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')">
        <Servo>
          <Index Type='Index'>7</Index>
          <name>INH</name>
          <vSource>74</vSource>
        </Servo>
      </xsl:if>

      <xsl:if test="(/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')">
        <Servo>
          <Index Type='Index'>8</Index>
          <name>INH</name>
          <vSource>74</vSource>
        </Servo>
      </xsl:if>

      <xsl:if test="$masterVolume">
        <Voice>
          <masterVolume><xsl:value-of select="$masterVolume" /></masterVolume>
        </Voice>
      </xsl:if>
    </SPM>
  </xsl:template>
  
  <!-- Generic cases must be defined first -->
  <xsl:template mode="namevalue" match="*[@Type='String']">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="Type">String</xsl:attribute>
      <xsl:value-of select="." />
    </xsl:element>
  </xsl:template>

  <xsl:template mode="namevalue" match="*[@Type='Object']">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="Type">Object</xsl:attribute>
      <xsl:apply-templates mode="namevalue" select="*" />
    </xsl:element>
  </xsl:template>

  <xsl:template mode="namevalue" match="*[@Type='Array']">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="Type">Array</xsl:attribute>
      <xsl:for-each select="Element">
        <Element><xsl:value-of select="text()" /></Element>
      </xsl:for-each>
    </xsl:element>
  </xsl:template>

  <xsl:template mode="top" match="*">
    <xsl:element name="{name(.)}">
      <xsl:apply-templates mode="namevalue" select="*" />
    </xsl:element>
  </xsl:template>
  
<!-- Sail -->
  
  <xsl:template match="ThroCurve" mode="top">
    <xsl:element name="{name(.)}">
      <xsl:choose>
        <xsl:when test="/SPM/Sail/Motor='None'">
          <analogID>64</analogID>
          <conditionID>0</conditionID>
        </xsl:when>
        <xsl:when test="/SPM/Sail/Motor='SpoilStk'">
          <analogID>64</analogID>
          <conditionID>145</conditionID>
          <assignedCurve Type='Array'>
            <xsl:call-template name="assignedCurveElements4">
              <xsl:with-param name="ap" select="/SPM/RAE-Mix/activePositions" />
            </xsl:call-template>
          </assignedCurve>
          <Curvedata Type='Object'>
            <Index Type='Index'>0</Index>
            <points>5</points>
            <Expo>Disabled</Expo>
            <trimActive>Disabled</trimActive>
            <curved>Enabled</curved>
            <X Type='Array'><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X>
            <Y Type='Array'><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>0</Element><Element>0</Element></Y>
          </Curvedata>
        </xsl:when>
        <xsl:when test="/SPM/Sail/Motor">
          <analogID><xsl:call-template name="subTypeC"><xsl:with-param name="sail" select="/SPM/Sail" /></xsl:call-template></analogID>
          <conditionID><xsl:call-template name="subTypeC"><xsl:with-param name="sail" select="/SPM/Sail" /></xsl:call-template></conditionID>
          <assignedCurve Type='Array'>
            <xsl:call-template name="assignedCurveElements3">
              <xsl:with-param name="ap" select="/SPM/RAE-Mix/activePositions" />
            </xsl:call-template>
          </assignedCurve>
          <Curvedata Type='Object'>
            <Index Type='Index'>0</Index>
            <points>5</points>
            <Expo>Disabled</Expo>
            <trimActive>Disabled</trimActive>
            <curved>Enabled</curved>
            <X Type='Array'><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X>
            <Y Type='Array'><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>0</Element><Element>0</Element></Y>
          </Curvedata>
          <Curvedata Type='Object'>
            <Index Type='Index'>1</Index>
            <points>5</points>
            <Expo>Disabled</Expo>
            <trimActive>Disabled</trimActive>
            <curved>Enabled</curved>
            <X Type='Array'><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X>
            <Y Type='Array'><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>0</Element><Element>0</Element></Y>
          </Curvedata>
        </xsl:when>
        <xsl:otherwise><xsl:apply-templates mode="namevalue" select="*" /></xsl:otherwise>
      </xsl:choose>
    </xsl:element>
  </xsl:template>

  <xsl:template mode="namevalue" match="ThroCurve/analogID">
    <analogID>
      <xsl:choose>
        <xsl:when test="/SPM/Sail">
          <xsl:call-template name="subTypeC">
            <xsl:with-param name="sail" select="/SPM/Sail" />
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates mode="mapvalue" select="." />
        </xsl:otherwise>
      </xsl:choose>
    </analogID>
  </xsl:template>
  
  <xsl:template mode="top" match="Sail">
    <xsl:element name="{name(.)}">
      <xsl:apply-templates mode="namevalue" select="Wing|Tail" />
      <xsl:choose>
        <xsl:when test="Motor='None'"><Motor>None</Motor></xsl:when>
        <xsl:otherwise>
          <Motor>Unsupported</Motor>
          <subTypeC><xsl:call-template name="subTypeC"><xsl:with-param name="sail" select="." /></xsl:call-template></subTypeC>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:element>
  </xsl:template>

  <xsl:template name="subTypeC">
    <xsl:param name="sail" />
    
    <xsl:choose>
      <xsl:when test="$sail/Motor='None'">64</xsl:when>
      <xsl:when test="$sail/Motor='SpoilStk'">64</xsl:when>
      <xsl:when test="$sail/Motor='Gear'">82</xsl:when>
      <xsl:when test="$sail/Motor='FModeSw'">83</xsl:when>
      <xsl:when test="$sail/Motor='EleDR'">84</xsl:when>
      <xsl:when test="$sail/Motor='Flap'">85</xsl:when>
      <xsl:when test="$sail/Motor='Aux2'">86</xsl:when>
      <xsl:when test="$sail/Motor='AilDR'">87</xsl:when>
      <xsl:when test="$sail/Motor='RudDR'">88</xsl:when>
      <xsl:when test="$sail/Motor='Mix'">89</xsl:when>
      <xsl:when test="$sail/Motor='Trainer'">92</xsl:when>
      <xsl:when test="$sail/Motor='Unsupported' and $sail/subTypeC='68'">112</xsl:when> <!-- LTrimD -->
      <xsl:when test="$sail/Motor='Unsupported' and $sail/subTypeC='69'">113</xsl:when> <!-- RTrimD -->
      <xsl:otherwise>UNKNOWN_<xsl:value-of select="$sail/Motor" /></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Named templates for assignedCurve elements (4-element, SpoilStk case) -->
  <xsl:template name="assignedCurveElements4">
    <xsl:param name="ap" />
    <xsl:choose>
      <xsl:when test="$ap='%0000'"><Element>0</Element><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0001'"><Element>1</Element><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0002'"><Element>0</Element><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0003'"><Element>1</Element><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0004'"><Element>0</Element><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0005'"><Element>1</Element><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0006'"><Element>0</Element><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0007'"><Element>1</Element><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0008'"><Element>0</Element><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%0009'"><Element>1</Element><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000A'"><Element>0</Element><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000B'"><Element>1</Element><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000C'"><Element>0</Element><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000D'"><Element>1</Element><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000E'"><Element>0</Element><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%000F'"><Element>1</Element><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
    </xsl:choose>
  </xsl:template>

  <!-- Named templates for assignedCurve elements (3-element, other motor case) -->
  <xsl:template name="assignedCurveElements3">
    <xsl:param name="ap" />
    <xsl:choose>
      <xsl:when test="$ap='%0000'"><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0001'"><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0002'"><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0003'"><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
      <xsl:when test="$ap='%0004'"><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%0005'"><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%0006'"><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
      <xsl:when test="$ap='%0007'"><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="namevalue" match="efItem/flapUp">
    <flapLeft><xsl:value-of select="." /></flapLeft>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="efItem/flapDown">
    <flapRight><xsl:value-of select="." /></flapRight>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="efItem/flonUp">
    <flonLeft><xsl:value-of select="." /></flonLeft>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="efItem/flonDown">
    <flonRight><xsl:value-of select="." /></flonRight>
  </xsl:template>
  
  <xsl:template mode="top" match="Differential">
    <xsl:choose>
      <xsl:when test="/SPM/Spektrum/Type='Sail'">
        <xsl:if test="ailRate">
          <Diff-Ail>
            <xsl:apply-templates mode="namevalue" select="conditionID" />
            <rate Type='Array'>
              <xsl:for-each select="ailRate/Element">
                <Element><xsl:value-of select="text()" /></Element>
              </xsl:for-each>
            </rate>
          </Diff-Ail>
        </xsl:if>
        <xsl:if test="flapRate">
          <Diff-Flap>
            <xsl:apply-templates mode="namevalue" select="conditionID" />
            <rate Type='Array'>
              <xsl:for-each select="flapRate/Element">
                <Element><xsl:value-of select="text()" /></Element>
              </xsl:for-each>
            </rate>
          </Diff-Flap>
        </xsl:if>
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="{name(.)}">
          <xsl:apply-templates mode="namevalue" select="*" />
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/cpItem[@Type='Object']">
    <efItem Type='Object'>
      <xsl:apply-templates mode="namevalue" select="*" />
    </efItem>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flap">
    <flapLeft><xsl:value-of select=".*10" /></flapLeft>
    <flapRight>
      <xsl:choose>
        <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_2'">
          <xsl:value-of select=".*-10" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select=".*10" />
        </xsl:otherwise>
      </xsl:choose>
    </flapRight>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flon">
    <flonLeft><xsl:value-of select=".*-10" /></flonLeft>
    <flonRight><xsl:value-of select=".*10" /></flonRight>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/elevator">
    <elevator><xsl:value-of select=".*10" /></elevator>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem[@Type='Object']">
    <efItem Type='Object'>
      <xsl:apply-templates mode="namevalue" select="*" />
    </efItem>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/conditionID">
    <conditionID>145</conditionID>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/offset">
    <offset><xsl:value-of select="-." /></offset>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/flapUp">
    <flapLeft>
      <xsl:choose>
        <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_1'">
          <xsl:value-of select="round(.*100 div 1024)*100" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="round(.*100 div 1024)*10" />
        </xsl:otherwise>
      </xsl:choose>
    </flapLeft>
  </xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem/flapDown">
    <flapRight>
      <xsl:choose>
        <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_1'">
          <xsl:value-of select="round(.*100 div 1024)*100" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="round(.*100 div 1024)*10" />
        </xsl:otherwise>
      </xsl:choose>
    </flapRight>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/flonUp">
    <flonLeft><xsl:value-of select="-round(.*100 div 1024)*10" /></flonLeft>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/flonDown">
    <flonRight><xsl:value-of select="-round(.*100 div 1024)*10" /></flonRight>
  </xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem/analogID">
    <analogID>
      <xsl:choose>
        <xsl:when test=".='0'">0</xsl:when> <!-- Inhibit -->
        <xsl:when test=".='16'">76</xsl:when> <!-- Spoiler Stick -->
        <xsl:otherwise><xsl:value-of select="." /> NOT SUPPORTED</xsl:otherwise>
      </xsl:choose>
    </analogID>
  </xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem/left">
    <left1><xsl:value-of select="." /></left1>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem/right">
    <right1><xsl:value-of select="." /></right1>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="FlpEleMix/analogID">
    <analogID>
      <xsl:choose>
        <xsl:when test=".='16'">198</xsl:when> <!-- Flap? -->
        <xsl:otherwise><xsl:value-of select="." /> NOT SUPPORTED</xsl:otherwise>
      </xsl:choose>
    </analogID>
  </xsl:template>
  
  <!-- Reverse RFL servo -->
  <xsl:template mode="namevalue" match="Servo/direction">
    <direction>
      <xsl:choose>
        <xsl:when test="/SPM/Sail and ../name='RFL'">
          <xsl:choose>
            <xsl:when test=".='Normal'">Reverse</xsl:when>
            <xsl:when test=".='Reverse'">Normal</xsl:when>
            <xsl:otherwise>UNKNOWN_<xsl:value-of select="." /></xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="text()" />
        </xsl:otherwise>
      </xsl:choose>
    </direction>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="Warning/Motor">
    <Motor>
      <xsl:choose>
        <xsl:when test="text()='%0000'">%0000</xsl:when>
        <xsl:when test="text()='%0001' and /SPM/Sail/Motor='SpoilStk'">%0000</xsl:when>
        <xsl:when test="text()='%0001'">
          <xsl:value-of select="/SPM/RAE-Mix/activePositions" />
        </xsl:when>
        <xsl:otherwise>UNKNOWN_<xsl:value-of select ="." /></xsl:otherwise>
      </xsl:choose>
    </Motor>
  </xsl:template>
  
<!-- Trainer -->
  <xsl:template mode="top" match="Trainer">
    <Trainer>
      <xsl:apply-templates mode="namevalue" select="Type" />
      <xsl:if test="Active">
        <mixOrNormal>%0000</mixOrNormal>
        <mixRatio Type='Array'>
          <xsl:for-each select="Active/Element">
            <Element>
              <xsl:choose>
                <xsl:when test="text()='Enabled'">100</xsl:when>
                <xsl:otherwise>0</xsl:otherwise>
              </xsl:choose>
            </Element>
          </xsl:for-each>
        </mixRatio>
      </xsl:if>
      <conditionID>92</conditionID>
      <MOverride>Disabled</MOverride>
      <activePositions>254</activePositions>
    </Trainer>
  </xsl:template>

  <!-- Use DX8 FMode switch -->
  <xsl:template mode="top" match="FMode">
    <FMode>
      <xsl:choose>
        <xsl:when test="/SPM/Heli">
          <switch_a><xsl:apply-templates mode="mapvalue" select="switch_a" /></switch_a>
          <switch_b>0</switch_b>
          <switch_c><xsl:apply-templates mode="mapvalue" select="switch_b" /></switch_c>
          <size>18</size>
          <fmtable Type='Array'>
            <Element>1</Element><Element>1</Element><Element>1</Element><Element>1</Element><Element>1</Element><Element>1</Element>
            <Element>2</Element><Element>2</Element><Element>2</Element><Element>2</Element><Element>2</Element><Element>2</Element>
            <Element>3</Element><Element>4</Element><Element>4</Element><Element>3</Element><Element>4</Element><Element>4</Element>
          </fmtable>
          <activePositions>%0006</activePositions>
        </xsl:when>
        <xsl:otherwise><xsl:apply-templates mode="namevalue" select="switch_a|switch_b|switch_c|size|data" /></xsl:otherwise>
      </xsl:choose>
    </FMode>
    <xsl:if test="/SPM/Sail">
      <FMode_Names>
        <fmName Type='Object'>
          <Index Type='Index'>0</Index>
          <display Type='String'>Launch</display>
          <fmVox>%0053</fmVox>
        </fmName>
        <fmName Type='Object'>
          <Index Type='Index'>1</Index>
          <display Type='String'>Cruise</display>
          <fmVox>%0054</fmVox>
        </fmName>
        <fmName Type='Object'>
          <Index Type='Index'>2</Index>
          <display Type='String'>Thermal</display>
          <fmVox>%0056</fmVox>
        </fmName>
        <fmName Type='Object'>
          <Index Type='Index'>3</Index>
          <display Type='String'>Speed</display>
          <fmVox>%0057</fmVox>
        </fmName>
        <fmName Type='Object'>
          <Index Type='Index'>4</Index>
          <display Type='String'>Land</display>
          <fmVox>%0055</fmVox>
        </fmName>
      </FMode_Names>
    </xsl:if>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="FMode/size">
    <size>18</size>
  </xsl:template>

  <xsl:template mode="namevalue" match="FMode/data[@Type='Array']">
    <fmtable Type='Array'>
      <xsl:for-each select="Element">
        <Element><xsl:value-of select="text()" /></Element>
        <xsl:if test="position() mod 3=0">
          <Element>0</Element>
          <Element>0</Element>
          <Element>0</Element>
        </xsl:if>
      </xsl:for-each>
    </fmtable>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="RAE-Mix/percentAileron">
    <xsl:element name="{name(.)}"><xsl:value-of select="text()" /></xsl:element>
    <xsl:element name="{concat(name(.), 'R')}"><xsl:value-of select="text()" /></xsl:element>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="RAE-Mix/percentElevator">
    <xsl:element name="{name(.)}"><xsl:value-of select="text()" /></xsl:element>
    <xsl:element name="{concat(name(.), 'R')}"><xsl:value-of select="-text()" /></xsl:element>
  </xsl:template>
  
  <xsl:template mode="top" match="C-Mix|S-Mix">
    <xsl:element name="{name(.)}">
      <xsl:apply-templates mode="namevalue" select="*" />
      <conditionID>145</conditionID>
    </xsl:element>
  </xsl:template>

  <xsl:template mode="namevalue" match="Warning/FltMode">
    <FltMode>
      <xsl:choose>
        <xsl:when test="/SPM/Sail">
          <xsl:value-of select="text()" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:choose>
            <xsl:when test="text()='%0000'">%0000</xsl:when>
            <xsl:when test="text()='%0020'">%0004</xsl:when>
            <xsl:when test="text()='%0040'">%0008</xsl:when>
            <xsl:when test="text()='%0060'">%000C</xsl:when>
            <xsl:when test="text()='%0080'">%0000</xsl:when>
            <xsl:when test="text()='%00A0'">%0004</xsl:when>
            <xsl:when test="text()='%00C0'">%0008</xsl:when>
            <xsl:when test="text()='%00E0'">%000C</xsl:when>
            <xsl:otherwise>UNKNOWN_<xsl:value-of select ="text()" /></xsl:otherwise>
          </xsl:choose>
        </xsl:otherwise>
      </xsl:choose>
    </FltMode>
    <xsl:if test="/SPM/Heli">
      <Hold>
        <xsl:choose>
          <xsl:when test="text()='%0000'">%0000</xsl:when>
          <xsl:when test="text()='%0020'">%0000</xsl:when>
          <xsl:when test="text()='%0040'">%0000</xsl:when>
          <xsl:when test="text()='%0060'">%0000</xsl:when>
          <xsl:when test="text()='%0080'">%0002</xsl:when>
          <xsl:when test="text()='%00A0'">%0002</xsl:when>
          <xsl:when test="text()='%00C0'">%0002</xsl:when>
          <xsl:when test="text()='%00E0'">%0002</xsl:when>
          <xsl:otherwise>UNKNOWN_<xsl:value-of select ="text()" /></xsl:otherwise>
        </xsl:choose>
      </Hold>
    </xsl:if>
  </xsl:template>

  <xsl:template mode="namevalue" match="Warning/Flaps">
    <Flaps>
      <xsl:choose>
        <xsl:when test="text()='%0000'">%0000</xsl:when>
        <xsl:when test="text()='%0001'">%0002</xsl:when>
        <xsl:when test="text()='%0002'">%0004</xsl:when>
        <xsl:when test="text()='%0003'">%0006</xsl:when>
        <xsl:when test="text()='%0004'">%0005</xsl:when>
        <xsl:otherwise>UNKNOWN_<xsl:value-of select ="text()" /></xsl:otherwise>
      </xsl:choose>
    </Flaps>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="*[@Type='Index']">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="Type">Index</xsl:attribute>
      <xsl:value-of select="." />
    </xsl:element>
  </xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/Generator">
    <Generator Type='String'><xsl:value-of select="$generator" /></Generator>
  </xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/VCode" />

  <xsl:template mode="namevalue" match="Spektrum/Name">
    <Name Type='String'>
      <xsl:choose>
        <xsl:when test="$modelName"><xsl:value-of select="$modelName" /></xsl:when>
        <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
      </xsl:choose>
    </Name>
  </xsl:template>

  <xsl:template mode="namevalue" match="Config/TrimType[text()='FMode']">
    <TrimType>%0000003F</TrimType>
  </xsl:template>

  <xsl:template mode="namevalue" match="Config/TrimType[text()='Common']">
    <TrimType>%00000000</TrimType>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="Config/FrameRate" />

  <!-- Spotted on DX7S 1.03. Not compatible with DX8. -->
  <xsl:template mode="namevalue" match="Timer/activePositions" />

  <!-- Possible servo remapping -->
  <xsl:template mode="namevalue" match="Servo/name">
    <name><xsl:value-of select="." /></name>
    <vSource>
      <xsl:choose>
        <xsl:when test="text()='LEL' and (/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')">8</xsl:when>
        <xsl:when test="text()='LRU' and (/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')">7</xsl:when>
        <xsl:when test="text()='MOT' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">6</xsl:when>
        <xsl:when test="text()='LAL' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">0</xsl:when>
        <xsl:when test="text()='RFL' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">4</xsl:when>
        <xsl:when test="text()='LFL' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">5</xsl:when>
        <xsl:otherwise><xsl:value-of select="../Index/text()" /></xsl:otherwise>
      </xsl:choose>
    </vSource>
  </xsl:template>
   
<!-- Invert subtrim when reversed for old versions -->
  <xsl:template mode="namevalue" match="Servo/subTrim">
    <subTrim>
      <xsl:choose>  
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="-text()" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="text()" />
        </xsl:otherwise>
      </xsl:choose>
    </subTrim>
  </xsl:template>

<!-- Swap and invert travelHigh when reversed for old versions -->
  <xsl:template mode="namevalue" match="Servo/travelLow">
    <travelLow>
      <xsl:choose>
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="-../travelHigh/text()" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="text()" />
        </xsl:otherwise>
      </xsl:choose>
    </travelLow>
  </xsl:template>

<!-- Swap and invert travelLow when reversed for old versions -->
  <xsl:template mode="namevalue" match="Servo/travelHigh">
    <travelHigh>
      <xsl:choose>
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="-../travelLow/text()" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="text()" />
        </xsl:otherwise>
      </xsl:choose>
    </travelHigh>
  </xsl:template>

<!-- Copy speed to downSpeed -->
  <xsl:template mode="namevalue" match="Servo/speed">
    <speed><xsl:value-of select="." /></speed>
    <speedDown><xsl:value-of select="." /></speedDown>
  </xsl:template>

  <!-- Default to Flight Mode -->
  <xsl:template mode="namevalue" match="RevoCurve/conditionID">
    <conditionID>145</conditionID>
  </xsl:template>

  <!-- Heli mixes default to using Flight Mode -->
  <xsl:template mode="namevalue" match="P-Mix/conditionID">
    <conditionID>
      <xsl:choose>
        <xsl:when test="/SPM/Spektrum/Type='Heli' and .='0' and ../activePositions!='%0000'">145</xsl:when>
        <xsl:otherwise><xsl:apply-templates mode="mapvalue" select="." /></xsl:otherwise>
      </xsl:choose>
    </conditionID>
  </xsl:template>

  <xsl:template mode="namevalue" match="conditionID|sourceID|outChan|analogID|StartID|switch_a|switch_b|switch_c|trimID|subTypeC">
    <xsl:element name="{name(.)}">
      <xsl:apply-templates mode="mapvalue" select="." />
    </xsl:element>
  </xsl:template>
  
  <!-- ID mapping: DX8 value → DX9 value. See DX9toDX8.xslt for the reverse mapping. -->
  <xsl:template mode="mapvalue" match="*">
    <xsl:choose>
      <xsl:when test="text()='0'">0</xsl:when>       <!-- Inhibit -->
      <xsl:when test="text()='1'">1</xsl:when>       <!-- THR Servo Out -->
      <xsl:when test="text()='2'">2</xsl:when>       <!-- AIL Servo Out -->
      <xsl:when test="text()='3'">3</xsl:when>       <!-- ELE Servo Out -->
      <xsl:when test="text()='4'">4</xsl:when>       <!-- RUD Servo Out -->
      <xsl:when test="text()='5'">5</xsl:when>       <!-- GER Servo Out -->
      <!-- <xsl:when test="text()='6' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">64</xsl:when> --> <!-- Sailplane LAL -->
      <xsl:when test="text()='6'">6</xsl:when>       <!-- AX1 Servo Out -->
      <xsl:when test="text()='7' and (/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')">9</xsl:when>   <!-- LEL Servo Out -->
      <xsl:when test="text()='7' and (/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')">8</xsl:when> <!-- LRU Servo Out -->
      <xsl:when test="text()='7' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">79</xsl:when> <!-- RFL / GER (analogID) -->
      <xsl:when test="text()='7' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1')">AX2_NOT_AVAILABLE</xsl:when> <!-- AX2 not available on DX9 -->
      <xsl:when test="text()='7'">7</xsl:when>       <!-- AX2 Servo Out -->
      <xsl:when test="text()='8'">8</xsl:when>       <!-- AX3 Servo Out -->
      <xsl:when test="text()='16' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">7</xsl:when> <!-- Sailplane MOT -->
      <xsl:when test="text()='16'">64</xsl:when>     <!-- Thr. Stick -->
      <xsl:when test="text()='17'">65</xsl:when>     <!-- Ail. Stick -->
      <xsl:when test="text()='18'">66</xsl:when>     <!-- Ele. Stick -->
      <xsl:when test="text()='19'">67</xsl:when>     <!-- Rud. Stick -->
      <xsl:when test="text()='20'">68</xsl:when>     <!-- 20-> 68  - Trainer->Switch I -->
      <xsl:when test="text()='21'">69</xsl:when>     <!-- R Knob -->
      <xsl:when test="text()='32'">78</xsl:when>     <!-- 32-> 78 - FLP (analogID) -->
      <!-- <xsl:when test="text()='33' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">6</xsl:when> --> <!-- Sailplane LFL -->
      <xsl:when test="text()='33'">79</xsl:when>     <!-- 33-> 79 - GER (analogID) -->
      <xsl:when test="text()='40'">82</xsl:when>     <!-- Gear -->
      <xsl:when test="text()='41'">83</xsl:when>     <!-- F Mode -->
      <xsl:when test="text()='42'">84</xsl:when>     <!-- Elev D/R -->
      <xsl:when test="text()='43'">85</xsl:when>     <!-- Flap -->
      <xsl:when test="text()='44'">86</xsl:when>     <!-- Aux 2 -->
      <xsl:when test="text()='45'">87</xsl:when>     <!-- Ail D/R -->
      <xsl:when test="text()='46'">88</xsl:when>     <!-- Rud D/R -->
      <xsl:when test="text()='47'">89</xsl:when>     <!-- Mix/Hold -->
      <xsl:when test="text()='50'">92</xsl:when>     <!-- Trainer/Bind -->
      <xsl:when test="text()='63'">107</xsl:when>    <!-- On -->
      <xsl:when test="text()='64'">108</xsl:when>    <!-- 64-> 108 - THR Trim (ThroCurve/trimID) -->
      <xsl:when test="text()='65'">109</xsl:when>    <!-- 65-> 109 - AIL Trim -->
      <xsl:when test="text()='66'">110</xsl:when>    <!-- 66-> 110 - AIL Trim -->
      <xsl:when test="text()='67'">111</xsl:when>    <!-- 67-> 111 - AIL Trim -->
      <xsl:when test="text()='68'">112</xsl:when>    <!-- LTrimD -->
      <xsl:when test="text()='69'">113</xsl:when>    <!-- RTrimD -->
      <xsl:when test="text()='70'">0</xsl:when>      <!-- 70-> 0   - FlpTrm (not supported on DX9?) -->
      <xsl:when test="text()='75'">0</xsl:when>      <!-- Knob: FlpTrm (not supported on DX9?) -->
      <xsl:when test="text()='242'">0</xsl:when>     <!-- 242->0   - Flaps (not supported on DX9?) -->
      <xsl:when test="text()='95'">127</xsl:when>    <!-- THR analogID -->
      <xsl:when test="text()='96'">128</xsl:when>    <!-- AIL analogID -->
      <xsl:when test="text()='97'">129</xsl:when>    <!-- ELE analogID -->
      <xsl:when test="text()='98'">130</xsl:when>    <!-- RUD analogID -->
      <xsl:when test="text()='127'">145</xsl:when>   <!-- F Mode -->
      <xsl:when test="text()='192' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">38</xsl:when> <!-- THR to AX2 -->
      <xsl:when test="text()='192'">32</xsl:when>    <!-- THR -->
      <xsl:when test="text()='193'">33</xsl:when>    <!-- AIL -->
      <xsl:when test="text()='194'">34</xsl:when>    <!-- ELE -->
      <xsl:when test="text()='195'">35</xsl:when>    <!-- RUD -->
      <xsl:when test="text()='196' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">37</xsl:when> <!-- LFL to AX1 -->
      <xsl:when test="text()='196'">36</xsl:when>    <!-- GER -->
      <xsl:when test="text()='197' and (/SPM/Sail/Wing/text()='Ail_2_Flap_1' or /SPM/Sail/Wing/text()='Ail_2_Flap_2')">32</xsl:when> <!-- AX1 to LAL -->
      <xsl:when test="text()='197'">37</xsl:when>    <!-- AX1 -->
      <xsl:when test="text()='198' and (/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')">40</xsl:when> <!-- LEL -->
      <xsl:when test="text()='198' and (/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')">39</xsl:when> <!-- LRU -->
      <xsl:when test="text()='198' and (/SPM/Sail/Wing/text()='Ail_2_Flap_2')">36</xsl:when> <!-- RFL -->
      <xsl:when test="text()='198'">38</xsl:when>    <!-- AX2 -->
      <xsl:when test="text()='199'">39</xsl:when>    <!-- AX3 -->
      <xsl:when test="text()='200'">52</xsl:when>    <!-- LFL (EF-Mix/outChan) -->
      <xsl:when test="text()='238'">34</xsl:when>    <!-- 238->34 - LAL (same as ELE?) -->
      <xsl:when test="text()='239'">35</xsl:when>    <!-- 239->35 - RUD (same as RUD?) -->
      <xsl:when test="text()='244'">200</xsl:when>   <!-- 244->200 - Gyro -->
      <xsl:when test="text()='245'">201</xsl:when>   <!-- 245->201 - Governor -->
      <xsl:otherwise>UNKNOWN_<xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  
<!-- AR-Mix values need to be inverted -->  
  <xsl:template mode="namevalue" match="AR-Mix/Curvedata/Y[@Type='Array']">
    <Y Type='Array'>
      <xsl:for-each select="Element">
        <Element><xsl:value-of select="-text()" /></Element>
      </xsl:for-each>
    </Y>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="*">
    <xsl:element name="{name(.)}">
      <xsl:value-of select="." />
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>
