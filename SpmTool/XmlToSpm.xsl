<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="text" omit-xml-declaration="yes" indent="no" />

  <xsl:template match="/SPM">
    <xsl:apply-templates mode="top" select="*" />

<xsl:text>

*EOF*
</xsl:text>

</xsl:template>

  <xsl:template mode="top" match="*">&lt;<xsl:value-of select="name(.)" />&gt;
<xsl:apply-templates mode="namevalue" select="*" />&lt;/<xsl:value-of select="name(.)" />&gt;

</xsl:template>

  <xsl:template mode="namevalue" match="*[@Type='Index']">
    <xsl:text>*</xsl:text><xsl:value-of select="name(.)" />=<xsl:value-of select="text()" />
<xsl:text>
</xsl:text>
</xsl:template>

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

  <xsl:template mode="namevalue" match="*">
    <xsl:value-of select="name(.)" />= <xsl:value-of select="." />
    <xsl:text>
</xsl:text>
</xsl:template>

</xsl:stylesheet>