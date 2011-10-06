LRIT XML Resources ReadMe file
===============================
Release Version: 1.3
Release Date:    07 Dec 2010
===============================


XSDs
----
File						Version	Date
------------------------------------------------------
DDP.xsd						1.2	24 Oct 2008
DDPNotification.xsd			1.0	22 Aug 2008
DDPRequest.xsd				1.1	24 Oct 2008
DDPUpdate.xsd				1.1	24 Sept 2008
JournalReport.xsd			1.0	22 Aug 2008
PricingFile.xsd				1.0	22 Aug 2008
PricingNotification.xsd		1.0	22 Aug 2008
PricingRequest.xsd			1.1	24 Oct 2008
PricingUpdate.xsd			1.1	24 Oct 2008
Receipt.xsd					1.0	22 Aug 2008
SARSURPICRequest.xsd		1.1	24 Oct 2008
ShipPositionReport.xsd		1.1	24 Oct 2008
ShipPositionRequest.xsd		1.0	22 Aug 2008
SystemStatus.xsd			1.0	22 Aug 2008
Types.xsd					1.2	07 Dec 2010


WSDLs
-----
File						Version	Date
------------------------------------------------------
DC.wsdl						1.0	22 Aug 2008
DDP.wsdl					1.0	22 Aug 2008
IDE-DC.wsdl					1.0	22 Aug 2008
IDE-DDP.wsdl				1.0	22 Aug 2008


NOTE: "Release Version" is incremented if any file within this
set is updated. This is the number that should be placed in the
schemaVersion attribute of all LRIT SOAP messages.

Each file in this set has its own "Version" number to track
changes to that file, independently of other files within the set. 
This number is not used in any LRIT SOAP message.


Changelog
---------
22 Aug 2008 - Release Version 1.0
 * First versioned release of the LRIT XML files.

24 Sept 2008 - Release Version 1.1
 * Updated DDP.xsd to:
	- Remove Exclusion as a Regular datatype
	- Add DDPServer and IDE as Immediate datatypes, and LRITCoordinator as Regular
	- Remove "minOccurs=0" on names of ports, facilities and places

 * Updated DDPUpdate.xsd to:
	- Add  ReferenceId element with reference message ID

24 Oct 2008 - Release Version 1.2
 * Updated DDP.xsd to:
	- Add "minOccurs=0" for Data Centre contact points
 * Updated DDPRequest.xsd to:
	- Add ReferenceId element with optional reference message ID
 * Updated PricingRequest.xsd to:
	- Add ReferenceId element with optional reference message ID
 * Updated PricingUpdate.xsd to:
	- Add ReferenceId element with optional reference message ID
 * Updated SARSURPICRequest.xsd to:
	- Restrict maximum number of positions to 4
	- Correct coordinate value patterns for circular and rectangular areas
 * Updated ShipPositionReport.xsd to:
	- Add optional CSPId element
 * Updated Types.xsd to:
	- Define CSP LRIT ID type for update to ShipPositionReport.xsd
	
07 Dec 2010 - Release Version 1.3
 * Updated Types.xsd to:
	- Changed percentageValueType restriction to "<xs:pattern value="[0-9]+(\.[0-9]{1,2})?"/>"
	- Changed priceValueType restriction to "<xs:pattern value="[0-9]+(\.[0-9]{1,4})?"/>"