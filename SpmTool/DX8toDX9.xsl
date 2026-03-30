<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:spm="http://mutantdesign.co.uk/spm">

	<xsl:output method="xml" omit-xml-declaration="yes" indent="no" />
  <xsl:param name="modelName" />
  <xsl:param name="generator" />
  <xsl:param name="masterVolume" />

  <!--
  <msxsl:script language="CSharp" implements-prefix="spm">
  <![CDATA[
    public int and(int x, int y) { return x & y; }
    public int or(int x, int y) { return x | y; }
  ]]>
  </msxsl:script>

  <xsl:value-of select="spm:and(@foo, @bar)" />
  <xsl:value-of select="spm:or(@foo, @bar)" />
  -->

  <xsl:template match="/SPM">
    <SPM>
    <xsl:apply-templates mode="top" select="Spektrum|Acro|Trim|Servo|DR_Expo|ThroCut|P-Mix|Timer|FMode|EF-Mix|AR-Mix|FlapSystem|Differential|ThroCurve|Special|SoftSw|TrimID|Telemetry|Trainer|Heli|PitchCurve|RevoCurve|Gyro|Governor|RAE-Mix|C-Mix|S-Mix|SwashPlate|Warning|Config|Sail|CamberPreset|CamberMix|FlpEleMix|AR-Mix-S|AF-Mix-S" />

<xsl:if test="(/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')"><Servo>
<Index Type="Index">7</Index>
<name>INH</name>
<vSource>74</vSource>
</Servo>
</xsl:if>

<xsl:if test="(/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')"><Servo>
<Index Type="Index">8</Index>
<name>INH</name>
<vSource>74</vSource>
</Servo>
</xsl:if>

<xsl:if test="$masterVolume"><Voice>
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
    <ThroCurve>
    <xsl:choose>
  <xsl:when test="/SPM/Sail/Motor='None'"><analogID>64</analogID>
<conditionID>0</conditionID></xsl:when>
  <xsl:when test="/SPM/Sail/Motor='SpoilStk'"><analogID>64</analogID>
<conditionID>145</conditionID>
<assignedCurve Type="Array"><xsl:choose>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0000'"><Element>0</Element><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0001'"><Element>1</Element><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0002'"><Element>0</Element><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0003'"><Element>1</Element><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0004'"><Element>0</Element><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0005'"><Element>1</Element><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0006'"><Element>0</Element><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0007'"><Element>1</Element><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0008'"><Element>0</Element><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0009'"><Element>1</Element><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000A'"><Element>0</Element><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000B'"><Element>1</Element><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000C'"><Element>0</Element><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000D'"><Element>1</Element><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000E'"><Element>0</Element><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%000F'"><Element>1</Element><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
</xsl:choose>
</assignedCurve>

<Curvedata Type="Object"><Index Type="Index">0</Index><points>5</points><Expo>Disabled</Expo><trimActive>Disabled</trimActive><curved>Enabled</curved><X Type="Array"><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X><Y Type="Array"><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>0</Element><Element>0</Element></Y></Curvedata></xsl:when>
  <xsl:when test="/SPM/Sail/Motor"><analogID><xsl:call-template name="subTypeC"><xsl:with-param name="sail" select="/SPM/Sail" /></xsl:call-template></analogID>
<conditionID><xsl:call-template name="subTypeC"><xsl:with-param name="sail" select="/SPM/Sail" /></xsl:call-template></conditionID>
<assignedCurve Type="Array"><xsl:choose>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0000'"><Element>0</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0001'"><Element>1</Element><Element>0</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0002'"><Element>0</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0003'"><Element>1</Element><Element>1</Element><Element>0</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0004'"><Element>0</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0005'"><Element>1</Element><Element>0</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0006'"><Element>0</Element><Element>1</Element><Element>1</Element></xsl:when>
  <xsl:when test="/SPM/RAE-Mix/activePositions='%0007'"><Element>1</Element><Element>1</Element><Element>1</Element></xsl:when>
</xsl:choose>
</assignedCurve>

<Curvedata Type="Object"><Index Type="Index">0</Index><points>5</points><Expo>Disabled</Expo><trimActive>Disabled</trimActive><curved>Enabled</curved><X Type="Array"><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X><Y Type="Array"><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>-1023</Element><Element>0</Element><Element>0</Element></Y></Curvedata><Curvedata Type="Object"><Index Type="Index">1</Index><points>5</points><Expo>Disabled</Expo><trimActive>Disabled</trimActive><curved>Enabled</curved><X Type="Array"><Element>-1023</Element><Element>-511</Element><Element>0</Element><Element>511</Element><Element>1023</Element><Element>0</Element><Element>0</Element></X><Y Type="Array"><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>1023</Element><Element>0</Element><Element>0</Element></Y></Curvedata></xsl:when>
      <xsl:otherwise><xsl:apply-templates mode="namevalue" select="*" /></xsl:otherwise>
    </xsl:choose>
    </ThroCurve>
  </xsl:template>

  <xsl:template mode="namevalue" match="ThroCurve/analogID">
    <xsl:choose>
      <xsl:when test="/SPM/Sail">
        <analogID><xsl:call-template name="subTypeC">
           <xsl:with-param name="sail" select="/SPM/Sail" />
        </xsl:call-template></analogID>
      </xsl:when>
      <xsl:otherwise>
        <analogID><xsl:apply-templates mode="mapvalue" select="." /></analogID>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="top" match="Sail">
    <Sail>
      <xsl:apply-templates mode="namevalue" select="Wing|Tail" />
    <xsl:choose>
      <xsl:when test="Motor='None'"><Motor>None</Motor></xsl:when>
      <xsl:otherwise><Motor>Unsupported</Motor>
<subTypeC><xsl:call-template name="subTypeC">
           <xsl:with-param name="sail" select="." />
        </xsl:call-template></subTypeC>
</xsl:otherwise>
    </xsl:choose>
    </Sail>
</xsl:template>

  <xsl:template name="subTypeC">
    <xsl:param name="sail" />
    <xsl:choose>
      <xsl:when test="$sail/Motor='Unsupported' and $sail/subTypeC">
        <xsl:apply-templates mode="mapvalue" select="$sail/subTypeC" />
      </xsl:when>
      <xsl:when test="$sail/Motor='Gear'">82</xsl:when>
      <xsl:when test="$sail/Motor='FModeSw'">83</xsl:when>
      <xsl:when test="$sail/Motor='EleDR'">84</xsl:when>
      <xsl:when test="$sail/Motor='Flap'">85</xsl:when>
      <xsl:when test="$sail/Motor='Aux2'">86</xsl:when>
      <xsl:when test="$sail/Motor='AilDR'">87</xsl:when>
      <xsl:when test="$sail/Motor='RudDR'">88</xsl:when>
      <xsl:when test="$sail/Motor='Mix'">89</xsl:when>
      <xsl:when test="$sail/Motor='Trainer'">92</xsl:when>
      <xsl:when test="$sail/Motor='SpoilStk'">64</xsl:when>
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
<xsl:if test="ailRate"><Diff-Ail>
<xsl:apply-templates mode="namevalue" select="conditionID" />
<rate Type="Array"><xsl:for-each select="ailRate/Element">
        <Element><xsl:value-of select="text()" /></Element>
    </xsl:for-each></rate>
</Diff-Ail>
</xsl:if>
<xsl:if test="flapRate"><Diff-Flap>
<xsl:apply-templates mode="namevalue" select="conditionID" />
<rate Type="Array"><xsl:for-each select="flapRate/Element">
        <Element><xsl:value-of select="text()" /></Element>
    </xsl:for-each></rate>
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
    <efItem Type="Object">
      <xsl:apply-templates mode="namevalue" select="*" />
    </efItem>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flap">
    <flapLeft><xsl:value-of select=".*10" /></flapLeft>
    <flapRight><xsl:choose>
    <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_2'">
      <xsl:value-of select=".*-10" />
    </xsl:when>
    <xsl:otherwise>
      <xsl:value-of select=".*10" />
    </xsl:otherwise>
  </xsl:choose></flapRight>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flon">
    <flonLeft><xsl:value-of select=".*-10" /></flonLeft>
    <flonRight><xsl:value-of select=".*10" /></flonRight>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/cpItem/elevator">
    <elevator><xsl:value-of select=".*10" /></elevator>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem[@Type='Object']">
    <efItem Type="Object">
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
    <flonLeft>
    <xsl:choose>
      <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_1'">
        <xsl:value-of select="round(.*-100 div 1024)*100" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="round(.*-100 div 1024)*10" />
      </xsl:otherwise>
    </xsl:choose>
    </flonLeft>
  </xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem/flonDown">
    <flonRight>
    <xsl:choose>
      <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_1'">
        <xsl:value-of select="round(.*-100 div 1024)*100" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="round(.*-100 div 1024)*10" />
      </xsl:otherwise>
    </xsl:choose>
    </flonRight>
  </xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem/analogID">
    <analogID><xsl:apply-templates mode="mapvalue" select="." /></analogID>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/conditionID">
    <conditionID>145</conditionID>
</xsl:template>

  <xsl:template mode="namevalue" match="EF-Mix/conditionID">
    <conditionID>145</conditionID>
</xsl:template>

  <xsl:template mode="namevalue" match="FlpEleMix/analogID">
    <analogID><xsl:apply-templates mode="mapvalue" select="." /></analogID>
</xsl:template>

  <xsl:template mode="namevalue" match="FlpEleMix/conditionID">
    <conditionID><xsl:choose>
      <xsl:when test="/SPM/Sail">145</xsl:when>
      <xsl:otherwise><xsl:value-of select="." /></xsl:otherwise>
    </xsl:choose></conditionID>
</xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/conditionID">
    <conditionID>145</conditionID>
</xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem[@Type='Object']">
    <arafItem Type="Object">
      <xsl:apply-templates mode="namevalue" select="Index" />
      <left>0</left>
      <right>0</right>
      <xsl:apply-templates mode="namevalue" select="left" />
      <xsl:apply-templates mode="namevalue" select="right" />
      <left2>0</left2>
      <right2>0</right2>
    </arafItem>
</xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem/left">
    <left1><xsl:value-of select="." /></left1>
</xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem/right">
    <right1><xsl:value-of select="." /></right1>
</xsl:template>

  <xsl:template mode="namevalue" match="AF-Mix-S/conditionID">
    <conditionID><xsl:choose>
      <xsl:when test="/SPM/Sail">107</xsl:when>
      <xsl:otherwise><xsl:value-of select="." /></xsl:otherwise>
    </xsl:choose></conditionID>
</xsl:template>

  <xsl:template mode="namevalue" match="AF-Mix-S/arafItem[@Type='Object']">
    <arafItem Type="Object">
      <xsl:apply-templates mode="namevalue" select="*" />
      <left1>0</left1>
      <right1>0</right1>
      <left2>0</left2>
      <right2>0</right2>
    </arafItem>
</xsl:template>

  <xsl:template mode="top" match="Trainer">
    <Trainer>
    <xsl:choose>
      <xsl:when test="Type/text()='Disabled'">
        <xsl:apply-templates mode="namevalue" select="Type" />
        <conditionID>92</conditionID>
        <MOverride>Disabled</MOverride>
        <activePositions>254</activePositions>
      </xsl:when>
      <xsl:when test="Type/text()='Normal'">
        <xsl:apply-templates mode="namevalue" select="Type" />
        <mixOrNormal>%0000</mixOrNormal>
        <conditionID>92</conditionID>
        <MOverride>Disabled</MOverride>
        <activePositions>254</activePositions>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="namevalue" select="*" />
      </xsl:otherwise>
    </xsl:choose>
    </Trainer>
</xsl:template>

  <xsl:template mode="top" match="FMode">
    <FMode>
    <xsl:choose>
      <xsl:when test="/SPM/Heli">
        <xsl:apply-templates mode="namevalue" select="*" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="namevalue" select="*" />
      </xsl:otherwise>
    </xsl:choose>
    </FMode>

    <xsl:if test="/SPM/Spektrum/Type='Sail'">
      <FMode_Names>
        <fmName Type="Object"><Index Type="Index">0</Index><display Type="String">Launch</display><fmVox>%0053</fmVox></fmName>
        <fmName Type="Object"><Index Type="Index">1</Index><display Type="String">Cruise</display><fmVox>%0054</fmVox></fmName>
        <fmName Type="Object"><Index Type="Index">2</Index><display Type="String">Thermal</display><fmVox>%0056</fmVox></fmName>
        <fmName Type="Object"><Index Type="Index">3</Index><display Type="String">Speed</display><fmVox>%0057</fmVox></fmName>
        <fmName Type="Object"><Index Type="Index">4</Index><display Type="String">Land</display><fmVox>%0055</fmVox></fmName>
      </FMode_Names>
    </xsl:if>
  </xsl:template>

  <xsl:template mode="namevalue" match="FMode/Config[@Type='Array']">
    <xsl:choose>
      <xsl:when test="/SPM/Heli">
        <Config Type="Array">
          <xsl:for-each select="Element">
            <Element>
              <xsl:choose>
                <xsl:when test="text()='%0000'">%0000003F</xsl:when>
                <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
              </xsl:choose>
            </Element>
          </xsl:for-each>
        </Config>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:copy-of select="@*" />
          <xsl:copy-of select="*" />
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="top" match="C-Mix|S-Mix">
    <xsl:element name="{name(.)}">
      <xsl:apply-templates mode="namevalue" select="*" />
    </xsl:element>
</xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/Generator">
    <Generator Type="String"><xsl:value-of select="$generator" /></Generator>
</xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/Name">
    <xsl:choose>
      <xsl:when test="$modelName">
        <Name Type="String"><xsl:value-of select="$modelName" /></Name>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:copy-of select="@*" />
          <xsl:copy-of select="*" />
          <xsl:value-of select="text()" />
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/VCode" />

  <xsl:template mode="namevalue" match="Config/FrameRate" />

  <xsl:template mode="namevalue" match="Config/TrimType">
    <TrimType><xsl:choose>
      <xsl:when test="text()='Common'">%00000000</xsl:when>
      <xsl:when test="text()='FMode'">%0000003F</xsl:when>
      <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose></TrimType>
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/FltMode">
    <FltMode><xsl:call-template name="convertWarnFltMode">
      <xsl:with-param name="fltmode" select="text()" />
    </xsl:call-template></FltMode>
</xsl:template>

  <xsl:template name="convertWarnFltMode">
    <xsl:param name="fltmode" />
    <xsl:variable name="tmp1">
      <xsl:choose>
        <xsl:when test="$fltmode='%0000'">%0000</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='8'">%0002</xsl:when>
        <xsl:otherwise>%0000</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="tmp2">
      <xsl:choose>
        <xsl:when test="substring($fltmode,5,1)='2'">%0004</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='A'">%0004</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='6'">%0004</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='E'">%0004</xsl:when>
        <xsl:otherwise>%0000</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="tmp3">
      <xsl:choose>
        <xsl:when test="substring($fltmode,5,1)='4'">%0008</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='C'">%0008</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='6'">%0008</xsl:when>
        <xsl:when test="substring($fltmode,5,1)='E'">%0008</xsl:when>
        <xsl:otherwise>%0000</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="result">
      <xsl:value-of select="substring($tmp1,2) + substring($tmp2,2) + substring($tmp3,2)" />
    </xsl:variable>
    <xsl:value-of select="concat('%',substring(concat('0000',$result),string-length($result)+1,4))" />
  </xsl:template>

  <xsl:template mode="namevalue" match="Warning/Hold">
    <xsl:variable name="hold" select="text()" />
    <FltMode><xsl:choose>
      <xsl:when test="$hold='%0000'">%0000</xsl:when>
      <xsl:when test="substring($hold,5,1)='1'">%0001</xsl:when>
      <xsl:when test="substring($hold,5,1)='2'">%0002</xsl:when>
      <xsl:when test="substring($hold,5,1)='4'">%0001</xsl:when>
      <xsl:when test="substring($hold,5,1)='8'">%0002</xsl:when>
      <xsl:otherwise>%0000</xsl:otherwise>
    </xsl:choose></FltMode>
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/Gear">
    <Gear><xsl:choose>
      <xsl:when test="/SPM/Sail and text()='%0001'">%0002</xsl:when>
      <xsl:when test="/SPM/Sail and text()='%0002'">%0004</xsl:when>
      <xsl:when test="/SPM/Sail and text()='%0003'">%0006</xsl:when>
      <xsl:when test="/SPM/Sail and text()='%0004'">%0005</xsl:when>
      <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose></Gear>
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/Flaps">
    <Flaps><xsl:choose>
      <xsl:when test="text()='%0000'">%0000</xsl:when>
      <xsl:when test="text()='%0001'">%0002</xsl:when>
      <xsl:when test="text()='%0002'">%0004</xsl:when>
      <xsl:when test="text()='%0003'">%0006</xsl:when>
      <xsl:when test="text()='%0004'">%0005</xsl:when>
      <xsl:otherwise>UNKNOWN_<xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose></Flaps>
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/Motor">
    <Motor><xsl:choose>
      <xsl:when test="/SPM/Sail/Motor='Aux2' and /SPM/RAE-Mix/activePositions='%0001' and text()='%0001'">%0001</xsl:when>
      <xsl:when test="/SPM/Sail/Motor='Aux2' and /SPM/RAE-Mix/activePositions='%0001' and text()='%0002'">%0002</xsl:when>
      <xsl:when test="/SPM/Sail/Motor='Aux2' and /SPM/RAE-Mix/activePositions='%0002' and text()='%0001'">%0000</xsl:when>
      <xsl:when test="/SPM/Sail/Motor='SpoilStk' and text()='%0001'">%0000</xsl:when>
      <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose></Motor>
</xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix/Curvedata/Y[@Type='Array']">
    <Y Type="Array">
      <xsl:for-each select="Element">
        <Element><xsl:value-of select="-text()" /></Element>
      </xsl:for-each>
    </Y>
</xsl:template>

  <xsl:template mode="namevalue" match="Servo[@Type='Object']">
    <xsl:variable name="index" select="Index[@Type='Index']/text()" />
    <xsl:variable name="name" select="name/text()" />
    <Servo Type="Object">
      <xsl:choose>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='AILE' and $index='1'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>198</vSource>
        </xsl:when>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='AILE' and $index='6'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>199</vSource>
        </xsl:when>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='LFL' and /SPM/Sail/Wing='Ail_2_Flap_1'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>199</vSource>
        </xsl:when>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='LFL' and /SPM/Sail/Wing='Ail_2_Flap_2'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>200</vSource>
        </xsl:when>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='RFL' and /SPM/Sail/Wing='Ail_2_Flap_1'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>201</vSource>
        </xsl:when>
        <xsl:when test="/SPM/Spektrum/Type='Sail' and $name='RFL' and /SPM/Sail/Wing='Ail_2_Flap_2'">
          <xsl:apply-templates mode="namevalue" select="*" />
          <vSource>202</vSource>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates mode="namevalue" select="*" />
        </xsl:otherwise>
      </xsl:choose>
    </Servo>
</xsl:template>

  <xsl:template mode="namevalue" match="Servo/direction">
    <direction><xsl:choose>
      <xsl:when test="/SPM/Spektrum/Type='Sail' and ../name/text()='RFL' and text()='Normal'">Reverse</xsl:when>
      <xsl:when test="/SPM/Spektrum/Type='Sail' and ../name/text()='RFL' and text()='Reverse'">Normal</xsl:when>
      <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose></direction>
</xsl:template>

  <xsl:template mode="namevalue" match="Servo/subTrim">
    <xsl:element name="{name(.)}">
      <xsl:choose>
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="-." />
        </xsl:when>
        <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
      </xsl:choose>
    </xsl:element>
</xsl:template>

  <xsl:template mode="namevalue" match="Servo/travelLow">
    <xsl:element name="{name(.)}">
      <xsl:choose>
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="../travelHigh/text()" />
        </xsl:when>
        <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
      </xsl:choose>
    </xsl:element>
</xsl:template>

  <xsl:template mode="namevalue" match="Servo/travelHigh">
    <xsl:element name="{name(.)}">
      <xsl:choose>
        <xsl:when test="((/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)) and ../direction='Reverse'">
          <xsl:value-of select="../travelLow/text()" />
        </xsl:when>
        <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
      </xsl:choose>
    </xsl:element>
</xsl:template>

  <xsl:template mode="namevalue" match="*">
    <xsl:element name="{name(.)}">
      <xsl:copy-of select="@*" />
      <xsl:choose>
        <xsl:when test="@Type='Object' or @Type='Array'">
          <xsl:apply-templates mode="namevalue" select="*" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="text()" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:element>
</xsl:template>

  <!-- Map DX8 analog values to DX9 -->
  <xsl:template mode="mapvalue" match="*">
    <xsl:choose>
      <xsl:when test="text()='0'">0</xsl:when>
      <xsl:when test="text()='1'">65</xsl:when>
      <xsl:when test="text()='2'">66</xsl:when>
      <xsl:when test="text()='3'">67</xsl:when>
      <xsl:when test="text()='4'">68</xsl:when>
      <xsl:when test="text()='16'">76</xsl:when>
      <xsl:when test="text()='17'">75</xsl:when>
      <xsl:when test="text()='18'">74</xsl:when>
      <xsl:when test="text()='19'">73</xsl:when>
      <xsl:when test="text()='20'">72</xsl:when>
      <xsl:when test="text()='21'">21 NOT SUPPORTED</xsl:when>
      <xsl:when test="text()='32'">64</xsl:when>
      <xsl:when test="text()='33'">69</xsl:when>
      <xsl:when test="text()='34'">70</xsl:when>
      <xsl:when test="text()='35'">71</xsl:when>
      <xsl:when test="text()='36'">77</xsl:when>
      <xsl:when test="text()='40'">82</xsl:when>
      <xsl:when test="text()='41'">83</xsl:when>
      <xsl:when test="text()='42'">84</xsl:when>
      <xsl:when test="text()='43'">85</xsl:when>
      <xsl:when test="text()='44'">86</xsl:when>
      <xsl:when test="text()='45'">87</xsl:when>
      <xsl:when test="text()='46'">88</xsl:when>
      <xsl:when test="text()='47'">89</xsl:when>
      <xsl:when test="text()='48'">90</xsl:when>
      <xsl:when test="text()='49'">91</xsl:when>
      <xsl:when test="text()='50'">92</xsl:when>
      <xsl:when test="text()='52'">93</xsl:when>
      <xsl:when test="text()='53'">94</xsl:when>
      <xsl:when test="text()='54'">95</xsl:when>
      <xsl:when test="text()='55'">96</xsl:when>
      <xsl:when test="text()='56'">97</xsl:when>
      <xsl:when test="text()='57'">98</xsl:when>
      <xsl:when test="text()='58'">99</xsl:when>
      <xsl:when test="text()='59'">100</xsl:when>
      <xsl:when test="text()='60'">101</xsl:when>
      <xsl:when test="text()='61'">102</xsl:when>
      <xsl:when test="text()='62'">103</xsl:when>
      <xsl:when test="text()='63'">104</xsl:when>
      <xsl:when test="text()='64'">105</xsl:when>
      <xsl:when test="text()='65'">106</xsl:when>
      <xsl:when test="text()='66'">107</xsl:when>
      <xsl:when test="text()='67'">108</xsl:when>
      <xsl:when test="text()='68'">112</xsl:when>
      <xsl:when test="text()='69'">113</xsl:when>
      <xsl:when test="text()='95'">127</xsl:when>
      <xsl:when test="text()='96'">128</xsl:when>
      <xsl:when test="text()='97'">129</xsl:when>
      <xsl:when test="text()='98'">130</xsl:when>
      <xsl:when test="text()='127'">198</xsl:when>
      <xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
