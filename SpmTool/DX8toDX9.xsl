<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:spm="http://mutantdesign.co.uk/spm">

	<xsl:output method="text" omit-xml-declaration="yes" indent="no" />
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
    <xsl:apply-templates mode="top" select="Spektrum|Acro|Trim|Servo|DR_Expo|ThroCut|P-Mix|Timer|FMode|EF-Mix|AR-Mix|FlapSystem|Differential|ThroCurve|Special|SoftSw|TrimID|Telemetry|Trainer|Heli|PitchCurve|RevoCurve|Gyro|Governor|RAE-Mix|C-Mix|S-Mix|SwashPlate|Warning|Config|Sail|CamberPreset|CamberMix|FlpEleMix|AR-Mix-S|AF-Mix-S" />

<xsl:if test="(/SPM/Spektrum/Generator/text()='DX7S' and /SPM/Acro/Tail/text()='Dual_Rud')">&lt;Servo&gt;
*Index= 7
name=INH
vSource= 74
&lt;/Servo&gt;
</xsl:if>
    
<xsl:if test="(/SPM/Acro/Tail/text()='Dual_Rud_Ele' or /SPM/Acro/Tail/text()='Dual_Ele')">&lt;Servo&gt;
*Index= 8
name=INH
vSource= 74
&lt;/Servo&gt;
</xsl:if>
   
<xsl:if test="$masterVolume">&lt;Voice&gt;
masterVolume= <xsl:value-of select="$masterVolume" />
&lt;/Voice&gt;
</xsl:if>
    
<xsl:text>

*EOF*
</xsl:text>

</xsl:template>
  
  <!-- Generic cases must be defined first -->
  <xsl:template mode="namevalue" match="*[@Type='String']">
    <xsl:value-of select="name(.)" />="<xsl:value-of select="." />"
</xsl:template>

  <xsl:template mode="namevalue" match="*[@Type='Object']">
[<xsl:value-of select="name(.)" />]
<xsl:apply-templates mode="namevalue" select="*" />[/<xsl:value-of select="name(.)" />]
</xsl:template>

  <xsl:template mode="namevalue" match="*[@Type='Array']">
    <xsl:value-of select="name(.)" />:<xsl:for-each select="Element">
      <xsl:text> </xsl:text>
      <xsl:value-of select="text()" />
    </xsl:for-each>
<xsl:text>
</xsl:text>
</xsl:template>

<xsl:template mode="top" match="*">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="*" />&lt;/<xsl:value-of select="name(.)" />&gt;

</xsl:template>

  <xsl:template name="emitAssignedCurve">
    <xsl:param name="positionCount" />
    <xsl:param name="activePositions" />

    <xsl:choose>
      <xsl:when test="$positionCount=4">
        <xsl:choose>
          <xsl:when test="$activePositions='%0000'">0 0 0 0</xsl:when>
          <xsl:when test="$activePositions='%0001'">1 0 0 0</xsl:when>
          <xsl:when test="$activePositions='%0002'">0 1 0 0</xsl:when>
          <xsl:when test="$activePositions='%0003'">1 1 0 0</xsl:when>
          <xsl:when test="$activePositions='%0004'">0 0 1 0</xsl:when>
          <xsl:when test="$activePositions='%0005'">1 0 1 0</xsl:when>
          <xsl:when test="$activePositions='%0006'">0 1 1 0</xsl:when>
          <xsl:when test="$activePositions='%0007'">1 1 1 0</xsl:when>
          <xsl:when test="$activePositions='%0008'">0 0 0 1</xsl:when>
          <xsl:when test="$activePositions='%0009'">1 0 0 1</xsl:when>
          <xsl:when test="$activePositions='%000A'">0 1 0 1</xsl:when>
          <xsl:when test="$activePositions='%000B'">1 1 0 1</xsl:when>
          <xsl:when test="$activePositions='%000C'">0 0 1 1</xsl:when>
          <xsl:when test="$activePositions='%000D'">1 0 1 1</xsl:when>
          <xsl:when test="$activePositions='%000E'">0 1 1 1</xsl:when>
          <xsl:when test="$activePositions='%000F'">1 1 1 1</xsl:when>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="$activePositions='%0000'">0 0 0</xsl:when>
          <xsl:when test="$activePositions='%0001'">1 0 0</xsl:when>
          <xsl:when test="$activePositions='%0002'">0 1 0</xsl:when>
          <xsl:when test="$activePositions='%0003'">1 1 0</xsl:when>
          <xsl:when test="$activePositions='%0004'">0 0 1</xsl:when>
          <xsl:when test="$activePositions='%0005'">1 0 1</xsl:when>
          <xsl:when test="$activePositions='%0006'">0 1 1</xsl:when>
          <xsl:when test="$activePositions='%0007'">1 1 1</xsl:when>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="emitCurvedata">
    <xsl:param name="index" />
    <xsl:param name="yValue" />

[Curvedata]
*Index= <xsl:value-of select="$index" />
points= 5
Expo=Disabled
trimActive=Disabled
curved=Enabled
X: -1023 -511 0 511 1023 0 0
Y: <xsl:value-of select="$yValue" /> <xsl:value-of select="$yValue" /> <xsl:value-of select="$yValue" /> <xsl:value-of select="$yValue" /> <xsl:value-of select="$yValue" /> 0 0
[/Curvedata]</xsl:template>

  <xsl:template name="emitServoSpeed">
    <xsl:param name="speed" />

speed= <xsl:value-of select="$speed" />
speedDown= <xsl:value-of select="$speed" />
  </xsl:template>

  <xsl:template name="emitNamedValueLine">
    <xsl:param name="name" />
    <xsl:param name="value" />

<xsl:value-of select="$name" />= <xsl:value-of select="$value" />
<xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template name="emitMirroredValueLines">
    <xsl:param name="name" />
    <xsl:param name="value" />
    <xsl:param name="mirroredValue" />

<xsl:value-of select="$name" /> =<xsl:value-of select="$value" />
<xsl:text>
</xsl:text>
<xsl:value-of select="$name" />R =<xsl:value-of select="$mirroredValue" />
<xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template name="emitTrainerMixRatio">
mixOrNormal=%0000
mixRatio:<xsl:for-each select="Active/Element">
<xsl:text>  </xsl:text>
  <xsl:choose>
    <xsl:when test="text()='Enabled'">100</xsl:when>
    <xsl:otherwise>0</xsl:otherwise>
  </xsl:choose>
</xsl:for-each>
<xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template name="emitTrainerFooter">conditionID= 92
MOverride=Disabled
activePositions= 254
</xsl:template>

  <xsl:template name="emitSailFModeNames">
      <xsl:text>&lt;FMode_Names&gt;
[fmName]
*Index= 0
display="Launch"
fmVox=%0053
[/fmName]

[fmName]
*Index= 1
display="Cruise"
fmVox=%0054
[/fmName]

[fmName]
*Index= 2
display="Thermal"
fmVox=%0056
[/fmName]

[fmName]
*Index= 3
display="Speed"
fmVox=%0057
[/fmName]

[fmName]
*Index= 4
display="Land"
fmVox=%0055
[/fmName]
&lt;/FMode_Names&gt;

</xsl:text>
  </xsl:template>

  <xsl:template name="emitDifferentialBlock">
    <xsl:param name="name" />
    <xsl:param name="rateElements" />

&lt;<xsl:value-of select="$name" />&gt;
<xsl:apply-templates mode="namevalue" select="conditionID" />
<xsl:text>rate: </xsl:text><xsl:for-each select="$rateElements">
      <xsl:text> </xsl:text>
      <xsl:value-of select="text()" />
    </xsl:for-each>
&lt;/<xsl:value-of select="$name" />&gt;

  </xsl:template>
  
<!-- Sail -->
  
  <xsl:template match="ThroCurve" mode="top">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:choose>
  <xsl:when test="/SPM/Sail/Motor='None'">analogID =64
conditionID =0</xsl:when>
  <xsl:when test="/SPM/Sail/Motor='SpoilStk'">analogID =64
conditionID =145
assignedCurve: <xsl:call-template name="emitAssignedCurve"><xsl:with-param name="positionCount" select="4" /><xsl:with-param name="activePositions" select="/SPM/RAE-Mix/activePositions" /></xsl:call-template>
  
<xsl:call-template name="emitCurvedata"><xsl:with-param name="index" select="0" /><xsl:with-param name="yValue" select="-1023" /></xsl:call-template></xsl:when>
  <xsl:when test="/SPM/Sail/Motor"><xsl:variable name="sailSubTypeC" select="spm:MapSailSubTypeC(/SPM/Sail/Motor/text(), /SPM/Sail/subTypeC/text())" />analogID =<xsl:value-of select="$sailSubTypeC" />
conditionID =<xsl:value-of select="$sailSubTypeC" />
assignedCurve: <xsl:call-template name="emitAssignedCurve"><xsl:with-param name="positionCount" select="3" /><xsl:with-param name="activePositions" select="/SPM/RAE-Mix/activePositions" /></xsl:call-template>
  
<xsl:call-template name="emitCurvedata"><xsl:with-param name="index" select="0" /><xsl:with-param name="yValue" select="-1023" /></xsl:call-template>

<xsl:call-template name="emitCurvedata"><xsl:with-param name="index" select="1" /><xsl:with-param name="yValue" select="1023" /></xsl:call-template></xsl:when>
      <xsl:otherwise><xsl:apply-templates mode="namevalue" select="*" /></xsl:otherwise>
    </xsl:choose>
&lt;/<xsl:value-of select="name(.)" />&gt;
<xsl:text>
  
</xsl:text>
  </xsl:template>

  <xsl:template mode="namevalue" match="ThroCurve/analogID">
    <xsl:choose>
      <xsl:when test="/SPM/Sail">
        <xsl:value-of select="name(.)" />= <xsl:value-of select="spm:MapSailSubTypeC(/SPM/Sail/Motor/text(), /SPM/Sail/subTypeC/text())" /><xsl:text>
</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="name(.)" />= <xsl:apply-templates mode="mapvalue" select="." /><xsl:text>
</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template mode="top" match="Sail">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="Wing|Tail" />
    <xsl:choose>
      <xsl:when test="Motor='None'">Motor=None</xsl:when>
      <xsl:otherwise>Motor=Unsupported
subTypeC=<xsl:value-of select="spm:MapSailSubTypeC(Motor/text(), subTypeC/text())" />
</xsl:otherwise>
    </xsl:choose>
&lt;/<xsl:value-of select="name(.)" />&gt;<xsl:text>

</xsl:text></xsl:template>
   
  <xsl:template mode="namevalue" match="efItem/flapUp|efItem/flapDown|efItem/flonUp|efItem/flonDown"><xsl:call-template name="emitNamedValueLine"><xsl:with-param name="name"><xsl:choose><xsl:when test="self::flapUp">flapLeft</xsl:when><xsl:when test="self::flapDown">flapRight</xsl:when><xsl:when test="self::flonUp">flonLeft</xsl:when><xsl:otherwise>flonRight</xsl:otherwise></xsl:choose></xsl:with-param><xsl:with-param name="value" select="." /></xsl:call-template></xsl:template>
  
<xsl:template mode="top" match="Differential">
  <xsl:choose>
    <xsl:when test="/SPM/Spektrum/Type='Sail'">
<xsl:if test="ailRate"><xsl:call-template name="emitDifferentialBlock"><xsl:with-param name="name" select="'Diff-Ail'" /><xsl:with-param name="rateElements" select="ailRate/Element" /></xsl:call-template></xsl:if> 
<xsl:if test="flapRate"><xsl:call-template name="emitDifferentialBlock"><xsl:with-param name="name" select="'Diff-Flap'" /><xsl:with-param name="rateElements" select="flapRate/Element" /></xsl:call-template></xsl:if> 
    </xsl:when>
    <xsl:otherwise>&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="*" />&lt;/<xsl:value-of select="name(.)" />&gt;

</xsl:otherwise>
  </xsl:choose>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberPreset/cpItem[@Type='Object']|CamberMix/csItem[@Type='Object']">
[efItem]
<xsl:apply-templates mode="namevalue" select="*" />[/efItem]
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flap">flapLeft= <xsl:value-of select=".*10" />
flapRight= <xsl:choose>
    <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_2'">
      <xsl:value-of select=".*-10" />
    </xsl:when>
    <xsl:otherwise>
      <xsl:value-of select=".*10" />
    </xsl:otherwise>
  </xsl:choose> 
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/flon">flonLeft= <xsl:value-of select=".*-10" />
flonRight= <xsl:value-of select=".*10" />
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberPreset/cpItem/elevator">elevator= <xsl:value-of select=".*10" />
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/conditionID">conditionID= 145
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/offset">offset= <xsl:value-of select="-." />
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/flapUp|CamberMix/csItem/flapDown">
    <xsl:text>flap</xsl:text>
    <xsl:choose>
      <xsl:when test="self::flapUp">Left</xsl:when>
      <xsl:otherwise>Right</xsl:otherwise>
    </xsl:choose>
    <xsl:text>= </xsl:text>
    <xsl:choose>
      <xsl:when test="/SPM/Sail/Wing='Ail_2_Flap_1'">
        <xsl:value-of select="round(.*100 div 1024)*100" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="round(.*100 div 1024)*10" />
      </xsl:otherwise>
    </xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="CamberMix/csItem/flonUp|CamberMix/csItem/flonDown">flon<xsl:choose>
    <xsl:when test="self::flonUp">Left</xsl:when>
    <xsl:otherwise>Right</xsl:otherwise>
  </xsl:choose>= <xsl:value-of select="-round(.*100 div 1024)*10" />
<xsl:text>
</xsl:text>
</xsl:template>

  <xsl:template mode="namevalue" match="CamberMix/csItem/analogID">
    <xsl:text>analogID= </xsl:text>
    <xsl:choose>
      <xsl:when test=".='0'">0</xsl:when> <!-- Inhibit -->
      <xsl:when test=".='16'">76</xsl:when> <!-- Spoiler Stick -->
      <xsl:otherwise><xsl:value-of select="." /> NOT SUPPORTED</xsl:otherwise>
    </xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template mode="namevalue" match="AR-Mix-S/arafItem/left|AR-Mix-S/arafItem/right"><xsl:value-of select="name(.)" />1= <xsl:value-of select="." />
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="FlpEleMix/analogID">
    <xsl:text>analogID= </xsl:text>
    <xsl:choose>
      <xsl:when test=".='16'">198</xsl:when> <!-- Flap? -->
      <xsl:otherwise><xsl:value-of select="." /> NOT SUPPORTED</xsl:otherwise>
    </xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>
  
  <!-- Reverse RFL servo -->
  <xsl:template mode="namevalue" match="Servo/direction">
    <xsl:value-of select="name(.)" />= <xsl:value-of select="spm:MapServoDirection(text(), ../name/text(), boolean(/SPM/Sail))" />
<xsl:text>
</xsl:text>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="Warning/Motor">
    <xsl:text>Motor=</xsl:text><xsl:value-of select="spm:MapWarningMotor(text(), /SPM/Sail/Motor/text(), /SPM/RAE-Mix/activePositions/text())" />
<xsl:text>
</xsl:text>
  </xsl:template>
  
<!-- Trainer -->
  <xsl:template mode="top" match="Trainer">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="Type" />
<xsl:if test="Active"><xsl:call-template name="emitTrainerMixRatio" /></xsl:if><xsl:call-template name="emitTrainerFooter" />
&lt;/<xsl:value-of select="name(.)" />&gt;

</xsl:template>

<!-- Enable telemetry -->
<!--
  <xsl:template mode="namevalue" match="Telemetry/FlightLog[@Type='Object']">
[<xsl:value-of select="name(.)" />]
<xsl:apply-templates mode="namevalue" select="*" />
sdEnabled= 1
[/<xsl:value-of select="name(.)" />]
</xsl:template>
-->  

  <!-- Use DX8 FMode switch -->
  <xsl:template mode="top" match="FMode">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:choose>
    <xsl:when test="/SPM/Heli"><xsl:variable name="mappedSwitchA"><xsl:apply-templates mode="mapvalue" select="switch_a" /></xsl:variable><xsl:variable name="mappedSwitchB"><xsl:apply-templates mode="mapvalue" select="switch_b" /></xsl:variable><xsl:value-of select="spm:BuildHeliFMode($mappedSwitchA, $mappedSwitchB)" /></xsl:when>
      <xsl:otherwise><xsl:apply-templates mode="namevalue" select="switch_a|switch_b|switch_c|size|data" /></xsl:otherwise>
    </xsl:choose>&lt;/<xsl:value-of select="name(.)" />&gt;<xsl:text>
    
</xsl:text>
    <xsl:if test="/SPM/Sail">
    <xsl:call-template name="emitSailFModeNames" />
    </xsl:if>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="FMode/size">
    <xsl:value-of select="name(.)" /> =18
</xsl:template>

  <xsl:template mode="namevalue" match="FMode/data[@Type='Array']">fmtable:<xsl:for-each select="Element">
      <xsl:text> </xsl:text>
      <xsl:value-of select="text()" />
      <xsl:if test="position() mod 3=0"> 0 0 0</xsl:if>
    </xsl:for-each>
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="RAE-Mix/percentAileron|RAE-Mix/percentElevator"><xsl:call-template name="emitMirroredValueLines"><xsl:with-param name="name" select="name(.)" /><xsl:with-param name="value" select="text()" /><xsl:with-param name="mirroredValue"><xsl:choose><xsl:when test="self::percentElevator"><xsl:value-of select="-text()" /></xsl:when><xsl:otherwise><xsl:value-of select="text()" /></xsl:otherwise></xsl:choose></xsl:with-param></xsl:call-template></xsl:template>

  <xsl:template mode="top" match="C-Mix|S-Mix">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="*" /><xsl:call-template name="emitNamedValueLine"><xsl:with-param name="name" select="'conditionID'" /><xsl:with-param name="value" select="145" /></xsl:call-template>
&lt;/<xsl:value-of select="name(.)" />&gt;
  
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/FltMode">FltMode=<xsl:value-of select="spm:MapWarningFltMode(text(), boolean(/SPM/Sail))" />
<xsl:text>
</xsl:text>
    <xsl:if test="/SPM/Heli">Hold=<xsl:value-of select="spm:MapWarningHold(text())" />
<xsl:text>
</xsl:text></xsl:if> 
</xsl:template>

  <xsl:template mode="namevalue" match="Warning/Flaps">Flaps=<xsl:value-of select="spm:MapWarningFlaps(text())" />
<xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="*[@Type='Index']">
    <xsl:text>*</xsl:text><xsl:value-of select="name(.)" />= <xsl:value-of select="." />
<xsl:text>
</xsl:text>
</xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/Generator">
    <xsl:value-of select="name(.)" />="<xsl:value-of select="$generator" />"
</xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/VCode"></xsl:template>

  <xsl:template mode="namevalue" match="Spektrum/Name">
    <xsl:choose>
      <xsl:when test="$modelName">
        <xsl:value-of select="name(.)" />="<xsl:value-of select="$modelName" />"</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="name(.)" />="<xsl:value-of select="text()" />"</xsl:otherwise>
    </xsl:choose>
    <xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template mode="namevalue" match="Config/TrimType"><xsl:value-of select="name(.)" />=<xsl:value-of select="spm:MapTrimType(text())" /><xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="namevalue" match="Config/FrameRate"></xsl:template>

  <!-- Spotted on DX7S 1.03. Not compatible with DX8. -->
  <xsl:template mode="namevalue" match="Timer/activePositions"></xsl:template>
<!--
  <xsl:template mode="namevalue" match="Timer/Audio">audioX=<xsl:choose>
  <xsl:when test="text()='Enabled'">%00F4</xsl:when>
  <xsl:when test="text()='Disabled'">%0080</xsl:when>
  <xsl:otherwise>UNKNOWN_<xsl:value-of select ="text()" /></xsl:otherwise>
</xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>
  
  <xsl:template mode="namevalue" match="Timer/Vibrate">vibeX=<xsl:choose>
  <xsl:when test="text()='Enabled'">%0020</xsl:when>
  <xsl:when test="text()='Disabled'">%0000</xsl:when>
  <xsl:otherwise>UNKNOWN_<xsl:value-of select ="text()" /></xsl:otherwise>
</xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>
-->  
  <!-- Possible servo remaping -->
  <xsl:template mode="namevalue" match="Servo/name">
    <xsl:value-of select="name(.)" />=<xsl:value-of select="." />
<xsl:text>
vSource=</xsl:text><xsl:value-of select="spm:MapServoVSource(text(), ../Index/text(), /SPM/Spektrum/Generator/text(), /SPM/Acro/Tail/text(), /SPM/Sail/Wing/text())" />
<xsl:text>
</xsl:text>
  </xsl:template>
   
<!-- Invert servo values when reversed for old DX8/DX7S versions -->
  <xsl:template mode="namevalue" match="Servo[direction='Reverse']/*[self::subTrim or self::travelLow or self::travelHigh]">
    <xsl:choose>
      <xsl:when test="(/SPM/Spektrum/Generator='DX8' and substring(/SPM/Spektrum/VCode/text(),2)&lt;2.05) or (/SPM/Spektrum/Generator='DX7S' and substring(/SPM/Spektrum/VCode/text(),2)&lt;1.02)">
        <xsl:choose>
          <xsl:when test="self::subTrim">
            <xsl:value-of select="name(.)" />= <xsl:value-of select="-text()" />
          </xsl:when>
          <xsl:when test="self::travelLow">
            <xsl:value-of select="name(.)" />= <xsl:value-of select="-../travelHigh/text()" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="name(.)" />= <xsl:value-of select="-../travelLow/text()" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="name(.)" />= <xsl:value-of select="text()" />
      </xsl:otherwise>
    </xsl:choose>
<xsl:text>
</xsl:text>
  </xsl:template>

<!-- Copy speed to downSpeed -->
  <xsl:template mode="namevalue" match="Servo/speed"><xsl:call-template name="emitServoSpeed"><xsl:with-param name="speed" select="." /></xsl:call-template><xsl:text>
</xsl:text>
  </xsl:template>

  <!-- Default to Flight Mode -->
  <xsl:template mode="namevalue" match="RevoCurve/conditionID"><xsl:call-template name="emitNamedValueLine"><xsl:with-param name="name" select="name(.)" /><xsl:with-param name="value" select="145" /></xsl:call-template></xsl:template>

  <!-- Heli mixes default to using Flight Mode -->
  <xsl:template mode="namevalue" match="P-Mix/conditionID">
    <xsl:choose>
      <xsl:when test="/SPM/Spektrum/Type='Heli' and .='0' and ../activePositions!='%0000'">
        <xsl:call-template name="emitNamedValueLine"><xsl:with-param name="name" select="name(.)" /><xsl:with-param name="value" select="145" /></xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:variable name="mappedValue"><xsl:apply-templates mode="mapvalue" select="." /></xsl:variable><xsl:call-template name="emitNamedValueLine"><xsl:with-param name="name" select="name(.)" /><xsl:with-param name="value" select="$mappedValue" /></xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template mode="namevalue" match="conditionID|sourceID|outChan|analogID|StartID|switch_a|switch_b|switch_c|trimID|subTypeC">
    <xsl:value-of select="name(.)" />
    <xsl:text>= </xsl:text>
    <xsl:apply-templates mode="mapvalue" select="." />
    <xsl:text>
</xsl:text>
</xsl:template>
  
  <xsl:template mode="mapvalue" match="*">
    <xsl:value-of select="spm:MapValue(text(), /SPM/Spektrum/Generator/text(), /SPM/Acro/Tail/text(), /SPM/Sail/Wing/text())" />
</xsl:template>
  
  
<!-- AR-Mix values need to be inverted -->  
  <xsl:template mode="namevalue" match="AR-Mix/Curvedata/Y[@Type='Array']">
    <xsl:value-of select="name(.)" />:<xsl:for-each select="Element">
      <xsl:text> </xsl:text>
      <xsl:value-of select="-text()" />
    </xsl:for-each>
<xsl:text>
</xsl:text>
</xsl:template>
  
<xsl:template mode="namevalue" match="*">
    <xsl:value-of select="name(.)" />=<xsl:value-of select="." />
    <xsl:text>
</xsl:text>
</xsl:template>

</xsl:stylesheet>