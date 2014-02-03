<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [
  <!ENTITY nbsp "&#x00A0;">
]>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:pdcalendar="urn:pdcalendar" xmlns:Ontranet.Library="urn:Ontranet.Library" xmlns:uForum="urn:uForum" xmlns:uForum.raw="urn:uForum.raw" xmlns:twitter.library="urn:twitter.library" xmlns:PS.XSLTsearch="urn:PS.XSLTsearch" xmlns:UCommentLibrary="urn:UCommentLibrary"
  exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets tagsLib BlogLibrary pdcalendar Ontranet.Library uForum uForum.raw twitter.library PS.XSLTsearch UCommentLibrary ">

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:param name="currentPage"/>
  <xsl:variable name="recordsPerPage" select="5"/>
  <xsl:variable name="pageNumber">
    <xsl:choose>
      <xsl:when test="umbraco.library:RequestQueryString('page') &lt;= 1 or string(umbraco.library:RequestQueryString('page')) = '' or string(umbraco.library:RequestQueryString('page')) = 'NaN'">1</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="umbraco.library:RequestQueryString('page')"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="rows" select="$currentPage/ancestor::* /descendant::BlogPost [@isDoc]/descendant-or-self::* [@isDoc and string(umbracoNaviHide) != '1']"/>
  <xsl:variable name="numberOfRecords" select="count($rows)"/>

  <xsl:template match="/">
    <xsl:variable name="images" select="4490"/>
    <xsl:variable name="mediaItems" select="umbraco.library:GetMedia($images, true())"/>

    <br/>
    <xsl:call-template name="pager"></xsl:call-template>
    <section class="jobs-posts">
      <xsl:for-each select="$rows">
        <xsl:sort select="postDate" order="descending"/>
        <xsl:if test="position() &gt; $recordsPerPage * number($pageNumber - 1) and position() &lt;= number($recordsPerPage * number($pageNumber - 1) + $recordsPerPage )">
          <article class="post">
            <xsl:if test="string-length(image) &gt; 0">
              <div class="img-holder">
                <img width="280" height="210" alt="{@nodeName}">
                  <xsl:attribute name="src">
                    /OntranetImageResizer.ashx?imageFile=<xsl:value-of select="umbraco.library:GetMedia(image, 'false')/umbracoFile"/>&amp;width=280&amp;height=0
                  </xsl:attribute>
                </img>
                <a href="{umbraco.library:NiceUrl(@id)}" class="btn-more">Job</a>
              </div>
            </xsl:if>
            <div class="textbox">
              <h2>
                <a>
                  <xsl:attribute name="href">
                    <xsl:value-of select="umbraco.library:NiceUrl(@id)"></xsl:value-of>
                  </xsl:attribute>
                  <xsl:choose>
                    <xsl:when test="string(pageTitle) != ''">
                      <xsl:value-of select="pageTitle" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="@nodeName"/>
                    </xsl:otherwise>
                  </xsl:choose>
                </a>
              </h2>
              <div class="meta">
                <xsl:variable name="forfatter" select="author"/>


                <xsl:for-each select="$mediaItems/Medarbejder[user=$forfatter]">
                  <xsl:variable name="empNodeName" select="@nodeName"/>
                  <xsl:variable name="picFile" select="uploadFile"/>

                  <div class="thumb" style="float:left; padding-right: 5px">
                    <img>
                      <xsl:attribute name="src">
                        <xsl:value-of select="$picFile"/>
                      </xsl:attribute>
                    </img>
                  </div>
                </xsl:for-each>


                <xsl:variable name="sql">
                  SELECT userName FROM dbo.umbracoUser WHERE id=<xsl:value-of select="$forfatter"/>
                </xsl:variable>
                <xsl:variable name="sag" select="Ontranet.Library:GetDataSet('umbracoDbDSN', $sql, 'sag')"/>

                <xsl:value-of select="$sag//sag/userName"/>:
                <time datetime="{umbraco.library:ShortDate(postDate)}">
                  <xsl:value-of select="umbraco.library:LongDate(postDate)"/>
                </time>

              </div>

              <xsl:value-of select="shortDescription" disable-output-escaping="yes"/>

              <a class="btn-more" style="margin-top: 10px; dispaly:block">

                <xsl:attribute name="href">
                  <xsl:value-of select="umbraco.library:NiceUrl(@id)"></xsl:value-of>
                </xsl:attribute>

                Læs mere
              </a>

            </div>
          </article>
        </xsl:if>
      </xsl:for-each>
    </section>
    <xsl:call-template name="pager"></xsl:call-template>
  </xsl:template>
  <xsl:template name="pager">
    <ul class="paging">
      <xsl:call-template name="for.loop">
        <xsl:with-param name="i">1</xsl:with-param>
        <xsl:with-param name="page" select="$pageNumber"></xsl:with-param>
        <xsl:with-param name="count" select="ceiling(count($rows) div $recordsPerPage)"></xsl:with-param>
      </xsl:call-template>
    </ul>
  </xsl:template>
  <xsl:template name="for.loop">
    <xsl:param name="i"/>
    <xsl:param name="count"/>
    <xsl:param name="page"/>
    <xsl:if test="$i &lt;= $count">
      <xsl:if test="$page != $i">
        <li>
          <a href="{umbraco.library:NiceUrl($currentPage/@id)}?page={$i}" >
            <xsl:value-of select="$i" />
          </a>
        </li>
      </xsl:if>
      <xsl:if test="$page = $i">
        <li class="active">
          <xsl:value-of select="$i" />
        </li>
      </xsl:if>
    </xsl:if>
    <xsl:if test="$i &lt;= $count">
      <xsl:call-template name="for.loop">
        <xsl:with-param name="i">
          <xsl:value-of select="$i + 1"/>
        </xsl:with-param>
        <xsl:with-param name="count">
          <xsl:value-of select="$count"/>
        </xsl:with-param>
        <xsl:with-param name="page">
          <xsl:value-of select="$page"/>
        </xsl:with-param>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>