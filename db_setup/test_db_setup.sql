USE [TestHandlerB2B]
GO
DELETE FROM [dbo].[Proforma_Owned_Item]
GO
DELETE FROM [dbo].[Proforma_Future_Item]
GO
DELETE FROM [dbo].[Offer_Item]
GO
DELETE FROM [dbo].[EAN]
GO
DELETE FROM [dbo].[Waybill]
GO
DELETE FROM [dbo].[Notes_Delivery]
GO
DELETE FROM [dbo].[Delivery]
GO
DELETE FROM [dbo].[Delivery_company]
GO
DELETE FROM [dbo].[Delivery_Status]
GO
DELETE FROM [dbo].[Calculated_Price]
GO
DELETE FROM [dbo].[Outside_Item]
GO
DELETE FROM [dbo].[Calculated_Credit_note_Price]
GO
DELETE FROM [dbo].[Credit_note_Items]
GO
DELETE FROM [dbo].[Request]
GO
DELETE FROM [dbo].[Request_status]
GO
DELETE FROM [dbo].[Selling_Price]
GO
DELETE FROM [dbo].[Purchase_Price]
GO
DELETE FROM [dbo].[User_client]
GO
DELETE FROM [dbo].[Proforma]
GO
DELETE FROM [dbo].[Offer]
GO
DELETE FROM [dbo].[Offer_status]
GO
DELETE FROM [dbo].[User_notification]
GO
DELETE FROM [dbo].[Object_type]
GO
DELETE FROM [dbo].[Note]
GO
DELETE FROM [dbo].[Action_Log]
GO
DELETE FROM [dbo].[Log_Type]
GO
DELETE FROM [dbo].[Item_owner]
GO
DELETE FROM [dbo].[Owned_Item]
GO
DELETE FROM [dbo].[Item]
GO
DELETE FROM [dbo].[Credit_note]
GO
DELETE FROM [dbo].[Invoice]
GO
DELETE FROM [dbo].[Taxes]
GO
DELETE FROM [dbo].[Payment_Status]
GO
DELETE FROM [dbo].[Payment_Method]
GO
DELETE FROM [dbo].[Currency_Value]
GO
DELETE FROM [dbo].[Currency_Name]
GO
DELETE FROM [dbo].[App_User]
GO
DELETE FROM [dbo].[Solo_User]
GO
DELETE FROM [dbo].[Org_User]
GO
DELETE FROM [dbo].[Organization]
GO
DELETE FROM [dbo].[Country]
GO
DELETE FROM [dbo].[Availability_Status]
GO
DELETE FROM [dbo].[User_role]
GO
SET IDENTITY_INSERT [dbo].[User_role] ON 
GO
INSERT [dbo].[User_role] ([role_id], [role_name]) VALUES (1, N'Admin')
GO
INSERT [dbo].[User_role] ([role_id], [role_name]) VALUES (2, N'Accountant')
GO
INSERT [dbo].[User_role] ([role_id], [role_name]) VALUES (3, N'Merchant')
GO
INSERT [dbo].[User_role] ([role_id], [role_name]) VALUES (4, N'Warehouse Manager')
GO
SET IDENTITY_INSERT [dbo].[User_role] OFF
GO
SET IDENTITY_INSERT [dbo].[Availability_Status] ON 
GO
INSERT [dbo].[Availability_Status] ([availability_status_id], [status_name], [days_for_realization]) VALUES (1, N'2-3 days', 3)
GO
INSERT [dbo].[Availability_Status] ([availability_status_id], [status_name], [days_for_realization]) VALUES (2, N'2 days', 2)
GO
INSERT [dbo].[Availability_Status] ([availability_status_id], [status_name], [days_for_realization]) VALUES (3, N'1 day', 1)
GO
INSERT [dbo].[Availability_Status] ([availability_status_id], [status_name], [days_for_realization]) VALUES (4, N'4 day', 4)
GO
SET IDENTITY_INSERT [dbo].[Availability_Status] OFF
GO
SET IDENTITY_INSERT [dbo].[Country] ON 
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (1, N'Afghanistan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (2, N'Albania')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (3, N'Algeria')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (4, N'Andorra')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (5, N'Anla')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (6, N'Antigua and Barbuda')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (7, N'Argentina')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (8, N'Armenia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (9, N'Australia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (10, N'Austria')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (11, N'Azerbaijan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (12, N'The Bahamas')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (13, N'Bahrain')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (14, N'Bangladesh')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (15, N'Barbados')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (16, N'Belarus')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (17, N'Belgium')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (18, N'Belize')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (19, N'Benin')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (20, N'Bhutan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (21, N'Bolivia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (22, N'Bosnia and Herzevina')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (23, N'Botswana')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (24, N'Brazil')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (25, N'Brunei')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (26, N'Bulgaria')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (27, N'Burkina Faso')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (28, N'Burundi')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (29, N'Cabo Verde')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (30, N'Cambodia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (31, N'Cameroon')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (32, N'Canada')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (33, N'Central African Republic')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (34, N'Chad')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (35, N'Chile')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (36, N'China')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (37, N'Colombia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (38, N'Comoros')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (39, N'Con, Democratic Republic of the')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (40, N'Con, Republic of the')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (41, N'Costa Rica')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (42, N'C?te d?Ivoire')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (43, N'Croatia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (44, N'Cuba')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (45, N'Cyprus')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (46, N'Czech Republic')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (47, N'Denmark')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (48, N'Djibouti')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (49, N'Dominica')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (50, N'Dominican Republic')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (51, N'East Timor (Timor-Leste)')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (52, N'Ecuador')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (53, N'Egypt')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (54, N'El Salvador')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (55, N'Equatorial Guinea')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (56, N'Eritrea')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (57, N'Estonia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (58, N'Eswatini')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (59, N'Ethiopia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (60, N'Fiji')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (61, N'Finland')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (62, N'France')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (63, N'Gabon')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (64, N'The Gambia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (65, N'Georgia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (66, N'Germany')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (67, N'Ghana')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (68, N'Greece')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (69, N'Grenada')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (70, N'Guatemala')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (71, N'Guinea')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (72, N'Guinea-Bissau')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (73, N'Guyana')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (74, N'Haiti')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (75, N'Honduras')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (76, N'Hungary')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (77, N'Iceland')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (78, N'India')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (79, N'Indonesia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (80, N'Iran')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (81, N'Iraq')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (82, N'Ireland')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (83, N'Israel')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (84, N'Italy')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (85, N'Jamaica')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (86, N'Japan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (87, N'Jordan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (88, N'Kazakhstan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (89, N'Kenya')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (90, N'Kiribati')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (91, N'Korea, North')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (92, N'Korea, South')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (93, N'Kosovo')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (94, N'Kuwait')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (95, N'Kyrgyzstan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (96, N'Laos')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (97, N'Latvia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (98, N'Lebanon')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (99, N'Lesotho')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (100, N'Liberia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (101, N'Libya')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (102, N'Liechtenstein')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (103, N'Lithuania')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (104, N'Luxembourg')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (105, N'Madagascar')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (106, N'Malawi')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (107, N'Malaysia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (108, N'Maldives')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (109, N'Mali')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (110, N'Malta')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (111, N'Marshall Islands')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (112, N'Mauritania')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (113, N'Mauritius')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (114, N'Mexico')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (115, N'Micronesia, Federated States of')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (116, N'Moldova')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (117, N'Monaco')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (118, N'Monlia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (119, N'Montenegro')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (120, N'Morocco')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (121, N'Mozambique')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (122, N'Myanmar (Burma)')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (123, N'Namibia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (124, N'Nauru')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (125, N'Nepal')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (126, N'Netherlands')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (127, N'New Zealand')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (128, N'Nicaragua')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (129, N'Niger')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (130, N'Nigeria')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (131, N'North Macedonia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (132, N'Norway')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (133, N'Oman')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (134, N'Pakistan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (135, N'Palau')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (136, N'Panama')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (137, N'Papua New Guinea')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (138, N'Paraguay')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (139, N'Peru')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (140, N'Philippines')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (141, N'Poland')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (142, N'Portugal')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (143, N'Qatar')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (144, N'Romania')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (145, N'Russia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (146, N'Rwanda')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (147, N'Saint Kitts and Nevis')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (148, N'Saint Lucia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (149, N'Saint Vincent and the Grenadines')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (150, N'Samoa')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (151, N'San Marino')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (152, N'Sao Tome and Principe')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (153, N'Saudi Arabia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (154, N'Senegal')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (155, N'Serbia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (156, N'Seychelles')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (157, N'Sierra Leone')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (158, N'Singapore')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (159, N'Slovakia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (160, N'Slovenia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (161, N'Solomon Islands')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (162, N'Somalia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (163, N'South Africa')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (164, N'Spain')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (165, N'Sri Lanka')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (166, N'Sudan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (167, N'Sudan, South')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (168, N'Suriname')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (169, N'Sweden')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (170, N'Switzerland')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (171, N'Syria')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (172, N'Taiwan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (173, N'Tajikistan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (174, N'Tanzania')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (175, N'Thailand')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (176, N'To')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (177, N'Tonga')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (178, N'Trinidad and Toba')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (179, N'Tunisia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (180, N'Turkey')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (181, N'Turkmenistan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (182, N'Tuvalu')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (183, N'Uganda')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (184, N'Ukraine')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (185, N'United Arab Emirates')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (186, N'United Kingdom')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (187, N'United States')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (188, N'Uruguay')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (189, N'Uzbekistan')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (190, N'Vanuatu')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (191, N'Vatican City')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (192, N'Venezuela')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (193, N'Vietnam')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (194, N'Yemen')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (195, N'Zambia')
GO
INSERT [dbo].[Country] ([country_id], [country_name]) VALUES (196, N'Zimbabwe')
GO
SET IDENTITY_INSERT [dbo].[Country] OFF
GO
SET IDENTITY_INSERT [dbo].[Organization] ON 
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (1, N'Test Handler B2B', NULL, N'ul. Test 23', N'Test', N'test postal', NULL, 141, 2)
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (2, N'Client A', NULL, N'ul. Client A', N'A', N'postal A', 10000, 44, 1)
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (3, N'Client B', 156165156, N'ul. Client B', N'B', N'postal B', 10000, 77, 2)
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (4, N'Client C', NULL, N'ul. Client C', N'C', N'postal C', 10000, 114, 4)
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (5, N'Client D', NULL, N'ul. D', N'D', N'D postal', 100000, 17, 1)
GO
INSERT [dbo].[Organization] ([organization_id], [org_name], [nip], [street], [city], [postal_code], [credit_limit], [country_id], [availability_status_id]) VALUES (6, N'Client to delete', NULL, N'ul. D', N'D', N'D postal', 100000, 17, 1)
GO
SET IDENTITY_INSERT [dbo].[Organization] OFF
GO
SET IDENTITY_INSERT [dbo].[Org_User] ON 
GO
INSERT [dbo].[Org_User] ([org_user_id], [role_id], [organizations_id]) VALUES (1, 1, 1)
GO
INSERT [dbo].[Org_User] ([org_user_id], [role_id], [organizations_id]) VALUES (2, 2, 1)
GO
INSERT [dbo].[Org_User] ([org_user_id], [role_id], [organizations_id]) VALUES (3, 3, 1)
GO
INSERT [dbo].[Org_User] ([org_user_id], [role_id], [organizations_id]) VALUES (4, 4, 1)
GO
SET IDENTITY_INSERT [dbo].[Org_User] OFF
GO
SET IDENTITY_INSERT [dbo].[App_User] ON 
GO
INSERT [dbo].[App_User] ([id_user], [email], [username], [surname], [solo_user_id], [org_user_id], [pass_hash], [pass_salt]) VALUES (1, N'test@handler.b2b.com', N'Test', N'Handler', NULL, 1, N'WutEIQAVJxdnu0cLWt7jEStx81Ss9KPT8Uou3cxHGqI=', N'6ErBjcASWA3FgRgc78iCPg==')
GO
INSERT [dbo].[App_User] ([id_user], [email], [username], [surname], [solo_user_id], [org_user_id], [pass_hash], [pass_salt]) VALUES (2, N'test.accountant@handler.b2b.com', N'Test', N'Accountant', NULL, 2, N'N79v3GAJrSEMAC2DTCzYUwbfK0OQrproHfrfwVPgdNE=', N'+OLkMUfK0aVbxNwxYlck2w==')
GO
INSERT [dbo].[App_User] ([id_user], [email], [username], [surname], [solo_user_id], [org_user_id], [pass_hash], [pass_salt]) VALUES (3, N'test.merchant@handler.b2b.com', N'Test', N'Merchant', NULL, 3, N'stMYARO7ZXImyZaFTNfH8eI6jNG/Sh6zXsv42w4t87s=', N'pUpzPVj97er737UT4x2KUw==')
GO
INSERT [dbo].[App_User] ([id_user], [email], [username], [surname], [solo_user_id], [org_user_id], [pass_hash], [pass_salt]) VALUES (4, N'test.w.manager@handler.b2b.com', N'Test', N'Manager', NULL, 4, N'KvQMwrJdrskF2AXh7AQpZcJX7Kove9Td7619ziezc/0=', N'1hOmgaoCwGpIJsF8raRItg==')
GO
SET IDENTITY_INSERT [dbo].[App_User] OFF
GO
INSERT [dbo].[Currency_Name] ([currency]) VALUES (N'EUR')
GO
INSERT [dbo].[Currency_Name] ([currency]) VALUES (N'PLN')
GO
INSERT [dbo].[Currency_Name] ([currency]) VALUES (N'USD')
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'PLN', CAST(N'2024-09-03T00:00:00.000' AS DateTime), CAST(1.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'EUR', CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(4.35 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'USD', CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(4.13 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'EUR', CAST(N'2024-11-22T00:00:00.000' AS DateTime), CAST(4.34 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'EUR', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(4.33 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'USD', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(4.13 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'EUR', CAST(N'2024-11-26T00:00:00.000' AS DateTime), CAST(4.32 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'USD', CAST(N'2024-11-26T00:00:00.000' AS DateTime), CAST(4.11 AS Decimal(5, 2)))
GO
INSERT [dbo].[Currency_Value] ([currency_name], [update_date], [currency_value]) VALUES (N'EUR', CAST(N'2024-11-29T00:00:00.000' AS DateTime), CAST(4.30 AS Decimal(5, 2)))
GO
SET IDENTITY_INSERT [dbo].[Payment_Method] ON 
GO
INSERT [dbo].[Payment_Method] ([payment_method_id], [method_name]) VALUES (1, N'Cash')
GO
INSERT [dbo].[Payment_Method] ([payment_method_id], [method_name]) VALUES (2, N'Credit card')
GO
INSERT [dbo].[Payment_Method] ([payment_method_id], [method_name]) VALUES (3, N'Transfer')
GO
INSERT [dbo].[Payment_Method] ([payment_method_id], [method_name]) VALUES (4, N'Blik')
GO
SET IDENTITY_INSERT [dbo].[Payment_Method] OFF
GO
SET IDENTITY_INSERT [dbo].[Payment_Status] ON 
GO
INSERT [dbo].[Payment_Status] ([payment_status_id], [status_name]) VALUES (1, N'Paid')
GO
INSERT [dbo].[Payment_Status] ([payment_status_id], [status_name]) VALUES (2, N'Unpaid')
GO
INSERT [dbo].[Payment_Status] ([payment_status_id], [status_name]) VALUES (3, N'Due to')
GO
SET IDENTITY_INSERT [dbo].[Payment_Status] OFF
GO
SET IDENTITY_INSERT [dbo].[Taxes] ON 
GO
INSERT [dbo].[Taxes] ([taxes_id], [tax_value]) VALUES (1, CAST(0.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Taxes] ([taxes_id], [tax_value]) VALUES (2, CAST(5.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Taxes] ([taxes_id], [tax_value]) VALUES (3, CAST(8.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Taxes] ([taxes_id], [tax_value]) VALUES (4, CAST(23.00 AS Decimal(5, 2)))
GO
SET IDENTITY_INSERT [dbo].[Taxes] OFF
GO
SET IDENTITY_INSERT [dbo].[Invoice] ON 
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (1, N'Purchase invoice A', 2, 1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), CAST(N'2024-12-26T00:00:00.000' AS DateTime), N'', 1, CAST(249.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Purchase_invoice_A_112_2024_11_261732635572740.pdf', 1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', 3, 2)
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (2, N'Purchase Invoice B', 4, 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(N'2024-12-05T00:00:00.000' AS DateTime), N'', 1, CAST(0.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Purchase_Invoice_B_114_2024_11_211732638874583.pdf', 1, CAST(N'2024-11-25T00:00:00.000' AS DateTime), N'EUR', 2, 1)
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (3, N'Sales Invoice A', 1, 4, CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(N'2024-12-05T00:00:00.000' AS DateTime), N'', 1, CAST(0.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Sales_Invoice_A_114_2024_11_211732643520043.pdf', 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), N'EUR', 2, 1)
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (4, N'Purchase invoice C', 3, 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(N'2024-12-05T00:00:00.000' AS DateTime), N'', 0, CAST(150.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Purchase_invoice_C_113_2024_11_211732645918046.pdf', 1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', 3, 2)
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (5, N'Sales Invoice C', 1, 3, CAST(N'2024-11-22T00:00:00.000' AS DateTime), CAST(N'2024-12-26T00:00:00.000' AS DateTime), N'', 0, CAST(250.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Sales_Invoice_C_113_2024_11_221732646008398.pdf', 1, CAST(N'2024-11-22T00:00:00.000' AS DateTime), N'EUR', 3, 2)
GO
INSERT [dbo].[Invoice] ([invoice_id], [invoice_number], [seller], [buyer], [invoice_date], [due_date], [note], [in_system], [transport_cost], [invoice_file_path], [taxes], [currency_value_date], [currency_name], [payment_method_id], [payments_status_Id]) VALUES (7, N'Purchase invoice D', 2, 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(N'2024-12-20T00:00:00.000' AS DateTime), N'', 1, CAST(250.00 AS Decimal(20, 2)), N'../../database/TestHandlerB2B/documents/Purchase_invoice_D_112_2024_11_211732974978289.pdf', 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), N'EUR', 3, 2)
GO
SET IDENTITY_INSERT [dbo].[Invoice] OFF
GO
SET IDENTITY_INSERT [dbo].[Credit_note] ON 
GO
INSERT [dbo].[Credit_note] ([id_credit_note], [credit_note_number], [credit_note_date], [in_system], [is_paid], [note], [invoice_id], [credit_file_path], [id_user]) VALUES (8, N'Credit A', CAST(N'2024-11-29' AS Date), 1, 1, N'', 2, N'../../database/TestHandlerB2B/documents/cn_Credit_A_1Test_Handler_B2BClient_C_2024_11_29_1732889632774.pdf', 1)
GO
INSERT [dbo].[Credit_note] ([id_credit_note], [credit_note_number], [credit_note_date], [in_system], [is_paid], [note], [invoice_id], [credit_file_path], [id_user]) VALUES (9, N'Credit B', CAST(N'2024-11-29' AS Date), 0, 1, N'', 1, N'../../database/TestHandlerB2B/documents/cn_Credit_B_1Test_Handler_B2BClient_A_2024_11_29_1732890994291.pdf', 1)
GO
INSERT [dbo].[Credit_note] ([id_credit_note], [credit_note_number], [credit_note_date], [in_system], [is_paid], [note], [invoice_id], [credit_file_path], [id_user]) VALUES (13, N'Credit C', CAST(N'2024-11-29' AS Date), 0, 0, N'', 4, N'../../database/TestHandlerB2B/documents/cn_Credit_C_1Test_Handler_B2BClient_B_2024_11_29_1732892942076.pdf', 1)
GO
INSERT [dbo].[Credit_note] ([id_credit_note], [credit_note_number], [credit_note_date], [in_system], [is_paid], [note], [invoice_id], [credit_file_path], [id_user]) VALUES (15, N'CreditS B', CAST(N'2024-11-29' AS Date), 0, 1, N'', 5, N'../../database/TestHandlerB2B/documents/cn_CreditS_B_1Test_Handler_B2BClient_B_2024_11_29_1732896359646.pdf', 1)
GO
INSERT [dbo].[Credit_note] ([id_credit_note], [credit_note_number], [credit_note_date], [in_system], [is_paid], [note], [invoice_id], [credit_file_path], [id_user]) VALUES (16, N'CreditS C', CAST(N'2024-11-30' AS Date), 0, 1, N'', 5, N'../../database/TestHandlerB2B/documents/cn_CreditS_C_1Test_Handler_B2BClient_B_2024_11_30_1732972199509.pdf', 1)
GO
SET IDENTITY_INSERT [dbo].[Credit_note] OFF
GO
SET IDENTITY_INSERT [dbo].[Item] ON 
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (1, N'Item A', N'Laptop.', N'itemA')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (2, N'Item B', N'Laptop.', N'itemB')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (3, N'Item C', N'Laptop.', N'itemC')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (4, N'Item D', N'Laptop.', N'itemD')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (5, N'Laptop Test 22"', N'Laptop.', N'LaptopTest022')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (6, N'Asus 27', N'', N'As234fds/234')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (7, N'Razer 4500x', N'', N'Raz45xi9/26b')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (8, N'Monitor', N'This is example', N'example1')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (9, N'Lamp', N'', N'lK4235')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (10, N'Knife', N'', N'k709142')
GO
INSERT [dbo].[Item] ([item_id], [item_name], [item_description], [part_number]) VALUES (11, N'NoRelation', N'', N'nr')
GO
SET IDENTITY_INSERT [dbo].[Item] OFF
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (1, 1)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (4, 1)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (4, 2)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (1, 3)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (4, 3)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (1, 4)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (2, 5)
GO
INSERT [dbo].[Owned_Item] ([invoice_id], [owned_item_id]) VALUES (7, 7)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 1, 1, 2)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 1, 3, 2)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 1, 4, 1)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 2, 5, 0)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 4, 1, 40)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 4, 2, 200)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 4, 3, 50)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (1, 7, 7, 15)
GO
INSERT [dbo].[Item_owner] ([id_user], [invoice_id], [owned_item_id], [qty]) VALUES (3, 7, 7, 5)
GO
SET IDENTITY_INSERT [dbo].[Log_Type] ON 
GO
INSERT [dbo].[Log_Type] ([log_type_id], [log_type_name]) VALUES (1, N'Create')
GO
INSERT [dbo].[Log_Type] ([log_type_id], [log_type_name]) VALUES (2, N'Modify')
GO
INSERT [dbo].[Log_Type] ([log_type_id], [log_type_name]) VALUES (3, N'Delete')
GO
SET IDENTITY_INSERT [dbo].[Log_Type] OFF
GO
SET IDENTITY_INSERT [dbo].[Note] ON 
GO
INSERT [dbo].[Note] ([note_id], [note_description], [note_date], [users_id]) VALUES (1, N'abc', CAST(N'2024-11-30T14:22:43.920' AS DateTime), 1)
GO
INSERT [dbo].[Note] ([note_id], [note_description], [note_date], [users_id]) VALUES (2, N'sth', CAST(N'2024-11-30T14:23:06.373' AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[Note] OFF
GO
SET IDENTITY_INSERT [dbo].[Object_type] ON 
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (1, N'User')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (2, N'Yours Proformas')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (3, N'Clients Proformas')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (4, N'Sales invoices')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (5, N'Yours invoices')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (6, N'Requests')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (7, N'Item')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (8, N'To client delivery')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (9, N'To user delivery')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (10, N'Role')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (11, N'Client')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (12, N'Pricelist')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (13, N'Yours credit notes')
GO
INSERT [dbo].[Object_type] ([object_type_id], [object_type_name]) VALUES (14, N'Client credit notes')
GO
SET IDENTITY_INSERT [dbo].[Object_type] OFF
GO
SET IDENTITY_INSERT [dbo].[User_notification] ON 
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (1, 2, N'Test Handler has created a new Sales invoices request for you.', 6, N'2', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (2, 2, N'Test Handler has created a new Clients Proformas request for you.', 6, N'3', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (3, 4, N'Test Handler has created a delivery with id 1.', 9, N'1', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (4, 4, N'Test Handler has created a delivery with id 2.', 8, N'2', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (5, 4, N'Test Handler has created a delivery with id 3.', 9, N'3', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (6, 1, N'Your import of outside items has started 30/11/2024 14:45.', 1, N'1', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (7, 1, N'Your import of outside items started 30/11/2024 14:45 has succeeded.', 1, N'1', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (8, 1, N'Import info. 4 rows has been omitted. Indexes: 7,8,9,10', 1, N'1', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (9, 1, N'The purchase invoice with number Purchase invoice D has been added by Test Accountant.', 5, N'7', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (10, 3, N'The item with id 7 has been bound to you by Test Accountant.', 7, N'7', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (11, 1, N'The item with id 7 has been bound to you by Test Accountant.', 7, N'7', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (12, 2, N'Test Merchant has created a new Yours invoices request for you.', 6, N'4', 1)
GO
INSERT [dbo].[User_notification] ([notification_id], [users_id], [info], [object_type_id], [reference], [is_read]) VALUES (13, 4, N'Test Merchant has created a delivery with id 4.', 9, N'4', 1)
GO
SET IDENTITY_INSERT [dbo].[User_notification] OFF
GO
SET IDENTITY_INSERT [dbo].[Offer_status] ON 
GO
INSERT [dbo].[Offer_status] ([offer_id], [status_name]) VALUES (1, N'Active')
GO
INSERT [dbo].[Offer_status] ([offer_id], [status_name]) VALUES (2, N'Deactivated')
GO
SET IDENTITY_INSERT [dbo].[Offer_status] OFF
GO
SET IDENTITY_INSERT [dbo].[Offer] ON 
GO
INSERT [dbo].[Offer] ([offer_id], [offer_name], [creation_date], [modification_date], [path_to_file], [offer_status_id], [max_qty], [currency_name], [id_user]) VALUES (1, N'Pricelist A', CAST(N'2024-11-30T14:29:15.343' AS DateTime), CAST(N'2024-11-30T14:29:15.343' AS DateTime), N'src/app/api/pricelist/TestHandlerB2B/Pricelist_A25EUR1732973355284.csv', 1, 25, N'EUR', 1)
GO
INSERT [dbo].[Offer] ([offer_id], [offer_name], [creation_date], [modification_date], [path_to_file], [offer_status_id], [max_qty], [currency_name], [id_user]) VALUES (2, N'Pricelist B', CAST(N'2024-11-30T14:38:21.717' AS DateTime), CAST(N'2024-11-30T14:38:21.717' AS DateTime), N'src/app/api/pricelist/TestHandlerB2B/Pricelist_B25PLN1732973901684.xlsx', 1, 25, N'PLN', 1)
GO
INSERT [dbo].[Offer] ([offer_id], [offer_name], [creation_date], [modification_date], [path_to_file], [offer_status_id], [max_qty], [currency_name], [id_user]) VALUES (3, N'Pricelist C', CAST(N'2024-11-30T14:38:21.717' AS DateTime), CAST(N'2024-11-30T14:38:21.717' AS DateTime), N'src/app/api/pricelist/TestHandlerB2B/Pricelist_C25PLN1732973901684.xlsx', 1, 25, N'PLN', 1)
GO
SET IDENTITY_INSERT [dbo].[Offer] OFF
GO
SET IDENTITY_INSERT [dbo].[Proforma] ON 
GO
INSERT [dbo].[Proforma] ([proforma_id], [proforma_number], [seller], [buyer], [proforma_date], [transport_cost], [note], [in_system], [proforma_file_path], [taxes], [payment_method_id], [currency_value_date], [currency_name], [id_user]) VALUES (1, N'Proforma A', 2, 1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), CAST(250.00 AS Decimal(6, 2)), N'', 1, N'../../database/TestHandlerB2B/documents/pr_Proforma_A_112_2024_11_2611732631976834.pdf', 1, 3, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', 1)
GO
INSERT [dbo].[Proforma] ([proforma_id], [proforma_number], [seller], [buyer], [proforma_date], [transport_cost], [note], [in_system], [proforma_file_path], [taxes], [payment_method_id], [currency_value_date], [currency_name], [id_user]) VALUES (2, N'Proforma B', 3, 1, CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(150.00 AS Decimal(6, 2)), N'', 1, N'../../database/TestHandlerB2B/documents/pr_Proforma_B_113_2024_11_2111732633802314.pdf', 1, 3, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', 1)
GO
INSERT [dbo].[Proforma] ([proforma_id], [proforma_number], [seller], [buyer], [proforma_date], [transport_cost], [note], [in_system], [proforma_file_path], [taxes], [payment_method_id], [currency_value_date], [currency_name], [id_user]) VALUES (3, N'ProformaS A', 1, 2, CAST(N'2024-11-30T00:00:00.000' AS DateTime), CAST(250.00 AS Decimal(6, 2)), N'', 0, N'../../database/TestHandlerB2B/documents/pr_ProformaS_A_112_2024_11_3011732972731630.pdf', 1, 3, CAST(N'2024-11-29T00:00:00.000' AS DateTime), N'EUR', 1)
GO
INSERT [dbo].[Proforma] ([proforma_id], [proforma_number], [seller], [buyer], [proforma_date], [transport_cost], [note], [in_system], [proforma_file_path], [taxes], [payment_method_id], [currency_value_date], [currency_name], [id_user]) VALUES (4, N'Proforma E', 4, 1, CAST(N'2024-11-30T00:00:00.000' AS DateTime), CAST(150.00 AS Decimal(6, 2)), N'', 0, N'../../database/TestHandlerB2B/documents/pr_Proforma_E_314_2024_11_3031732978234968.pdf', 1, 3, CAST(N'2024-11-29T00:00:00.000' AS DateTime), N'EUR', 3)
GO
INSERT [dbo].[Proforma] ([proforma_id], [proforma_number], [seller], [buyer], [proforma_date], [transport_cost], [note], [in_system], [proforma_file_path], [taxes], [payment_method_id], [currency_value_date], [currency_name], [id_user]) VALUES (5, N'Proforma to delete', 2, 1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), CAST(250.00 AS Decimal(6, 2)), N'', 1, N'../../database/TestHandlerB2B/documents/pr_Proforma_A_112_2024_11_2611732631976834.pdf', 1, 3, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', 1)
GO
SET IDENTITY_INSERT [dbo].[Proforma] OFF
GO
INSERT [dbo].[User_client] ([users_id], [organization_id]) VALUES (1, 2)
GO
INSERT [dbo].[User_client] ([users_id], [organization_id]) VALUES (1, 3)
GO
INSERT [dbo].[User_client] ([users_id], [organization_id]) VALUES (1, 4)
GO
INSERT [dbo].[User_client] ([users_id], [organization_id]) VALUES (3, 4)
GO
INSERT [dbo].[User_client] ([users_id], [organization_id]) VALUES (3, 5)
GO
SET IDENTITY_INSERT [dbo].[Purchase_Price] ON 
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (1, 5, CAST(647.36 AS Decimal(20, 2)), 1, 1)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (2, 2, CAST(1078.93 AS Decimal(20, 2)), 3, 1)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (3, 1, CAST(3840.97 AS Decimal(20, 2)), 4, 1)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (4, 1, CAST(2165.95 AS Decimal(20, 2)), 5, 2)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (5, 200, CAST(41.07 AS Decimal(20, 2)), 1, 4)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (6, 400, CAST(61.61 AS Decimal(20, 2)), 2, 4)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (7, 50, CAST(106.75 AS Decimal(20, 2)), 3, 4)
GO
INSERT [dbo].[Purchase_Price] ([purchase_price_id], [qty], [price], [owned_item_id], [invoice_id]) VALUES (8, 20, CAST(1086.73 AS Decimal(20, 2)), 7, 7)
GO
SET IDENTITY_INSERT [dbo].[Purchase_Price] OFF
GO
SET IDENTITY_INSERT [dbo].[Selling_Price] ON 
GO
INSERT [dbo].[Selling_Price] ([selling_price_id], [sell_invoice_id], [purchase_price_id], [id_user], [qty], [price]) VALUES (1, 3, 1, 1, 5, CAST(199.99 AS Decimal(20, 2)))
GO
INSERT [dbo].[Selling_Price] ([selling_price_id], [sell_invoice_id], [purchase_price_id], [id_user], [qty], [price]) VALUES (2, 5, 6, 1, 200, CAST(25.99 AS Decimal(20, 2)))
GO
INSERT [dbo].[Selling_Price] ([selling_price_id], [sell_invoice_id], [purchase_price_id], [id_user], [qty], [price]) VALUES (3, 5, 5, 1, 150, CAST(15.00 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Selling_Price] OFF
GO
SET IDENTITY_INSERT [dbo].[Request_status] ON 
GO
INSERT [dbo].[Request_status] ([request_status_id], [status_name]) VALUES (1, N'Fulfilled')
GO
INSERT [dbo].[Request_status] ([request_status_id], [status_name]) VALUES (2, N'Request cancelled')
GO
INSERT [dbo].[Request_status] ([request_status_id], [status_name]) VALUES (3, N'In progress')
GO
SET IDENTITY_INSERT [dbo].[Request_status] OFF
GO
SET IDENTITY_INSERT [dbo].[Request] ON 
GO
INSERT [dbo].[Request] ([request_id], [id_user_creator], [id_user_receiver], [request_status_id], [object_type_id], [filePath], [note], [title], [creation_date]) VALUES (1, 1, 1, 1, 5, N'../../database/TestHandlerB2B/documents/req_11Yoursinvoices1732972387411.pdf', N'abc
[Fulfilled] 30/11/2024 14:13
', N'Request A', CAST(N'2024-11-30T14:13:07.520' AS DateTime))
GO
INSERT [dbo].[Request] ([request_id], [id_user_creator], [id_user_receiver], [request_status_id], [object_type_id], [filePath], [note], [title], [creation_date]) VALUES (2, 1, 2, 2, 4, N'../../database/TestHandlerB2B/documents/req_21Salesinvoices1732972763814.pdf', N'afasf
[Request cancelled] 30/11/2024 15:09
', N'Request B', CAST(N'2024-11-30T14:19:23.843' AS DateTime))
GO
INSERT [dbo].[Request] ([request_id], [id_user_creator], [id_user_receiver], [request_status_id], [object_type_id], [filePath], [note], [title], [creation_date]) VALUES (3, 1, 2, 1, 3, N'../../database/TestHandlerB2B/documents/req_21ClientsProformas1732972797347.pdf', N'abc
[Fulfilled] 30/11/2024 15:09
', N'Request C', CAST(N'2024-11-30T14:19:57.383' AS DateTime))
GO
INSERT [dbo].[Request] ([request_id], [id_user_creator], [id_user_receiver], [request_status_id], [object_type_id], [filePath], [note], [title], [creation_date]) VALUES (4, 3, 2, 3, 5, N'../../database/TestHandlerB2B/documents/req_23Yoursinvoices1732976893097.pdf', N'pls add', N'Request E', CAST(N'2024-11-30T15:28:13.253' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Request] OFF
GO
SET IDENTITY_INSERT [dbo].[Credit_note_Items] ON 
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (8, 8, 4, -1, CAST(2165.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (9, 9, 3, 1, CAST(3672.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (10, 9, 3, -1, CAST(3844.80 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (16, 13, 5, -20, CAST(41.10 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (18, 15, 5, 20, CAST(54.25 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (19, 15, 5, -20, CAST(65.10 AS Decimal(20, 2)))
GO
INSERT [dbo].[Credit_note_Items] ([credit_item_id], [credit_note_id], [purchase_price_id], [qty], [new_price]) VALUES (20, 16, 5, 10, CAST(65.10 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Credit_note_Items] OFF
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-22T00:00:00.000' AS DateTime), 18, CAST(12.50 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-22T00:00:00.000' AS DateTime), 19, CAST(15.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-22T00:00:00.000' AS DateTime), 20, CAST(15.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-25T00:00:00.000' AS DateTime), 8, CAST(500.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 9, CAST(850.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 10, CAST(890.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'EUR', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 16, CAST(9.51 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'USD', CAST(N'2024-11-25T00:00:00.000' AS DateTime), 8, CAST(524.21 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'USD', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 9, CAST(893.43 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'USD', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 10, CAST(935.47 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Credit_note_Price] ([currency_name], [update_date], [credit_item_id], [price]) VALUES (N'USD', CAST(N'2024-11-26T00:00:00.000' AS DateTime), 16, CAST(10.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Outside_Item] ([item_id], [organization_id], [purchase_price], [qty], [currency_name]) VALUES (6, 2, CAST(500.00 AS Decimal(20, 2)), 2, N'EUR')
GO
INSERT [dbo].[Outside_Item] ([item_id], [organization_id], [purchase_price], [qty], [currency_name]) VALUES (7, 2, CAST(250.00 AS Decimal(20, 2)), 10, N'EUR')
GO
INSERT [dbo].[Outside_Item] ([item_id], [organization_id], [purchase_price], [qty], [currency_name]) VALUES (8, 2, CAST(350.00 AS Decimal(20, 2)), 20, N'EUR')
GO
INSERT [dbo].[Outside_Item] ([item_id], [organization_id], [purchase_price], [qty], [currency_name]) VALUES (9, 2, CAST(150.00 AS Decimal(20, 2)), 50, N'EUR')
GO
INSERT [dbo].[Outside_Item] ([item_id], [organization_id], [purchase_price], [qty], [currency_name]) VALUES (10, 2, CAST(270.99 AS Decimal(20, 2)), 3, N'EUR')
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(150.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (1, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(157.61 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (2, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(250.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (2, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(262.68 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (3, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(890.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (3, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(935.16 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (4, CAST(N'2024-11-25T00:00:00.000' AS DateTime), N'EUR', CAST(500.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (4, CAST(N'2024-11-25T00:00:00.000' AS DateTime), N'USD', CAST(524.48 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (5, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(9.52 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (5, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(10.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (6, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(14.28 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (6, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(15.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (7, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'EUR', CAST(24.73 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (7, CAST(N'2024-11-26T00:00:00.000' AS DateTime), N'USD', CAST(25.99 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (8, CAST(N'2024-11-21T00:00:00.000' AS DateTime), N'EUR', CAST(250.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Calculated_Price] ([purchase_price_id], [update_date], [currency_name], [price]) VALUES (8, CAST(N'2024-11-21T00:00:00.000' AS DateTime), N'USD', CAST(262.93 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Delivery_Status] ON 
GO
INSERT [dbo].[Delivery_Status] ([delivery_status_id], [status_name]) VALUES (1, N'Fulfilled')
GO
INSERT [dbo].[Delivery_Status] ([delivery_status_id], [status_name]) VALUES (2, N'In transport')
GO
INSERT [dbo].[Delivery_Status] ([delivery_status_id], [status_name]) VALUES (3, N'Delivered with issues')
GO
INSERT [dbo].[Delivery_Status] ([delivery_status_id], [status_name]) VALUES (4, N'Preparing')
GO
INSERT [dbo].[Delivery_Status] ([delivery_status_id], [status_name]) VALUES (5, N'Rejected')
GO
SET IDENTITY_INSERT [dbo].[Delivery_Status] OFF
GO
SET IDENTITY_INSERT [dbo].[Delivery_company] ON 
GO
INSERT [dbo].[Delivery_company] ([delivery_company_id], [delivery_company_name]) VALUES (1, N'DPD')
GO
SET IDENTITY_INSERT [dbo].[Delivery_company] OFF
GO
SET IDENTITY_INSERT [dbo].[Delivery] ON 
GO
INSERT [dbo].[Delivery] ([delivery_id], [estimated_delivery_date], [delivery_date], [delivery_status_id], [proforma_id], [delivery_company_id]) VALUES (1, CAST(N'2024-11-30T00:00:00.000' AS DateTime), NULL, 2, 1, 1)
GO
INSERT [dbo].[Delivery] ([delivery_id], [estimated_delivery_date], [delivery_date], [delivery_status_id], [proforma_id], [delivery_company_id]) VALUES (2, CAST(N'2024-11-30T00:00:00.000' AS DateTime), CAST(N'2024-11-30T15:58:34.647' AS DateTime), 1, 3, 1)
GO
INSERT [dbo].[Delivery] ([delivery_id], [estimated_delivery_date], [delivery_date], [delivery_status_id], [proforma_id], [delivery_company_id]) VALUES (3, CAST(N'2024-11-30T00:00:00.000' AS DateTime), CAST(N'2024-11-30T15:58:28.633' AS DateTime), 3, 2, 1)
GO
SET IDENTITY_INSERT [dbo].[Delivery] OFF
GO
INSERT [dbo].[Notes_Delivery] ([delivery_id], [note_id]) VALUES (1, 1)
GO
INSERT [dbo].[Notes_Delivery] ([delivery_id], [note_id]) VALUES (2, 2)
GO
SET IDENTITY_INSERT [dbo].[Waybill] ON 
GO
INSERT [dbo].[Waybill] ([waybill_id], [waybill], [deliveries_id]) VALUES (1, N'56156156156156151', 1)
GO
INSERT [dbo].[Waybill] ([waybill_id], [waybill], [deliveries_id]) VALUES (2, N'561561561651561', 2)
GO
INSERT [dbo].[Waybill] ([waybill_id], [waybill], [deliveries_id]) VALUES (3, N'6519684198165', 3)
GO
SET IDENTITY_INSERT [dbo].[Waybill] OFF
GO
INSERT [dbo].[EAN] ([ean], [item_id]) VALUES (N'1256156153132', 1)
GO
INSERT [dbo].[EAN] ([ean], [item_id]) VALUES (N'515616123132', 2)
GO
INSERT [dbo].[EAN] ([ean], [item_id]) VALUES (N'125324532545', 4)
GO
INSERT [dbo].[EAN] ([ean], [item_id]) VALUES (N'214235325235', 4)
GO
INSERT [dbo].[EAN] ([ean], [item_id]) VALUES (N'156156156', 5)
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (1, 1, CAST(89.33 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (1, 2, CAST(17.14 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (1, 3, CAST(164.84 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (1, 4, CAST(1050.20 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (2, 1, CAST(385.52 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (2, 2, CAST(73.93 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (2, 3, CAST(640.27 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (2, 4, CAST(3956.20 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (3, 1, CAST(385.52 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (3, 2, CAST(73.93 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (3, 3, CAST(640.27 AS Decimal(20, 2)))
GO
INSERT [dbo].[Offer_Item] ([offer_id], [item_id], [selling_price]) VALUES (3, 4, CAST(3956.20 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Proforma_Future_Item] ON 
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (1, 1, 1, 2, CAST(150.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (2, 1, 3, 5, CAST(350.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (3, 1, 4, 1, CAST(890.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (4, 2, 3, 20, CAST(50.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (5, 2, 4, 50, CAST(25.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (6, 2, 1, 70, CAST(25.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (7, 4, 6, 15, CAST(180.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (8, 4, 4, 5, CAST(160.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (9, 5, 1, 2, CAST(150.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (10, 5, 3, 5, CAST(350.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Future_Item] ([proforma_future_item_id], [proforma_id], [item_id], [qty], [purchase_price]) VALUES (11, 5, 4, 1, CAST(890.00 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Proforma_Future_Item] OFF
GO
SET IDENTITY_INSERT [dbo].[Proforma_Owned_Item] ON 
GO
INSERT [dbo].[Proforma_Owned_Item] ([proforma_owned_item_id], [proforma_id], [purchase_price_id], [qty], [selling_price]) VALUES (1, 3, 1, 2, CAST(169.99 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Owned_Item] ([proforma_owned_item_id], [proforma_id], [purchase_price_id], [qty], [selling_price]) VALUES (2, 3, 5, 40, CAST(15.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[Proforma_Owned_Item] ([proforma_owned_item_id], [proforma_id], [purchase_price_id], [qty], [selling_price]) VALUES (3, 3, 6, 200, CAST(25.99 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Proforma_Owned_Item] OFF
GO
