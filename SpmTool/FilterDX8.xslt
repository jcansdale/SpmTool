<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <!-- TODO: Should convert Digital -->
  <xsl:template match="/SPM/Digital"></xsl:template>

  <!-- TODO: Should convert Analog -->
  <xsl:template match="/SPM/Analog"></xsl:template>
  
  <!-- Conversion back to DX7S not supported  -->
  <xsl:template match="/SPM/Spektrum/Generator/text()">
    <xsl:choose>
      <xsl:when test=".='DX7S'">DX8</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Only found in one DX7s model -->
  <xsl:template match="/SPM/Timer/activePositions"></xsl:template>

  <!-- DX7s has only 2 flight modes -->
  <xsl:template match="/SPM/FMode/data/Element[7]/text()">
    <xsl:choose>
      <xsl:when test="/SPM/Spektrum/Generator='DX7S' and .='2'">3</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/SPM/Spektrum/VCode"></xsl:template>

  <!-- This gets changed from 0 to 127 (FMode) -->
  <xsl:template match="/SPM/RevoCurve/conditionID"></xsl:template>

  <!-- This gets commented out for DX9 -->
  <xsl:template match="/SPM/Config/FrameRate"></xsl:template>

  <!-- Don't know how these are different -->
  <xsl:template match="/SPM/Servo/sourceID/text()">
    <xsl:choose>
      <xsl:when test=".=238">194</xsl:when>
      <xsl:when test=".=239">195</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/SPM/SoftSw/sourceID/text()">
    <xsl:choose>
      <xsl:when test=".=70">0</xsl:when> <!-- FlpTrm (not supported on DX9?) -->
      <xsl:when test=".=242">0</xsl:when> <!-- Flaps (not supported on DX9?) -->
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/SPM/FlapSystem/trimID/text()">
    <xsl:choose>
      <xsl:when test=".=75">0</xsl:when>
      <!-- Knob: FlpTrm (not supported on DX9?)  -->
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
  
</xsl:stylesheet>
