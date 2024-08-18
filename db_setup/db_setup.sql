
-- Tables create

CREATE TABLE Action_Log (
    log_id int  NOT NULL IDENTITY(1, 1),
    log_description varchar(500)  NOT NULL,
    log_date date  NOT NULL,
    users_id int  NOT NULL,
    log_type_id int  NOT NULL,
    CONSTRAINT Action_Log_pk PRIMARY KEY  (log_id)
);

CREATE TABLE App_User (
    id_user int  NOT NULL IDENTITY(1, 1),
    email varchar(350)  NOT NULL,
    username varchar(250)  NOT NULL,
    surname varchar(250)  NOT NULL,
    solo_user_id int  NULL,
    org_user_id int  NULL,
    pass_hash varchar(200)  NOT NULL,
    pass_salt varchar(200)  NOT NULL,
    CONSTRAINT App_User_pk PRIMARY KEY  (id_user)
);

CREATE TABLE Availability_Status (
    availability_status_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(150)  NOT NULL,
    days_for_realization int  NOT NULL,
    CONSTRAINT Availability_Status_pk PRIMARY KEY  (availability_status_id)
);

CREATE TABLE Availability_Status_Organization (
    availability_status_id int  NOT NULL,
    organization_id int  NOT NULL,
    CONSTRAINT Availability_Status_Organization_pk PRIMARY KEY  (availability_status_id,organization_id)
);

CREATE TABLE Country (
    country_id int  NOT NULL IDENTITY(1, 1),
    country_name varchar(70)  NOT NULL,
    CONSTRAINT Country_pk PRIMARY KEY  (country_id)
);

CREATE TABLE Credit_note (
    id_credit_note int  NOT NULL IDENTITY(1, 1),
    credit_note_date date  NOT NULL,
    in_system bit  NOT NULL,
    note varchar(500)  NOT NULL,
    invoice_id int  NOT NULL,
    CONSTRAINT Credit_note_pk PRIMARY KEY  (id_credit_note)
);

CREATE TABLE Credit_note_Items (
    credit_note_id int  NOT NULL,
    owned_item_id int  NOT NULL,
    invoice_id int  NOT NULL,
    qty int  NOT NULL,
    amount decimal(20,2)  NOT NULL,
    CONSTRAINT Credit_note_Items_pk PRIMARY KEY  (credit_note_id,owned_item_id,invoice_id)
);

CREATE TABLE Currency_Name (
    curenncy varchar(5)  NOT NULL,
    CONSTRAINT Currency_Name_pk PRIMARY KEY  (curenncy)
);

CREATE TABLE Currency_Value (
    currency_name varchar(5)  NOT NULL,
    update_date datetime  NOT NULL,
    currency_value decimal(5,2)  NOT NULL,
    CONSTRAINT Currency_Value_pk PRIMARY KEY  (update_date,currency_name)
);

CREATE TABLE Currency_Value_Offer (
    offer_id int  NOT NULL,
    currancy_name varchar(5)  NOT NULL,
    curency_date datetime  NOT NULL,
    CONSTRAINT Currency_Value_Offer_pk PRIMARY KEY  (offer_id,currancy_name)
);

CREATE TABLE Delivery (
    delivery_id int  NOT NULL IDENTITY(1, 1),
    estimated_delivery_date datetime  NOT NULL,
    delivery_date datetime  NOT NULL,
    delivery_status_id int  NOT NULL,
    proforma_id int  NOT NULL,
    invoice_id int  NULL,
    CONSTRAINT Delivery_pk PRIMARY KEY  (delivery_id)
);

CREATE TABLE Delivery_Status (
    delivery_status_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(30)  NOT NULL,
    CONSTRAINT Delivery_Status_pk PRIMARY KEY  (delivery_status_id)
);

CREATE TABLE Delivery_company (
    delivery_company_id int  NOT NULL IDENTITY(1, 1),
    delivery_company_name varchar(40)  NOT NULL,
    CONSTRAINT Delivery_company_pk PRIMARY KEY  (delivery_company_id)
);

CREATE TABLE EAN (
    ean int  NOT NULL,
    item_id int  NOT NULL,
    CONSTRAINT EAN_pk PRIMARY KEY  (ean)
);

CREATE TABLE Invoice (
    invoice_id int  NOT NULL IDENTITY(1, 1),
    invoice_number varchar(40)  NOT NULL,
    seller int  NOT NULL,
    buyer int  NOT NULL,
    invoice_date datetime  NOT NULL,
    due_date datetime  NOT NULL,
    note varchar(500)  NOT NULL,
    in_system bit  NOT NULL,
    transport_cost decimal(6,2)  NOT NULL,
    invoice_file_path varchar(500)  NULL,
    taxes int  NOT NULL,
    currency_value_date datetime  NOT NULL,
    currency_name varchar(5)  NOT NULL,
    payment_method_id int  NOT NULL,
    payments_status_Id int  NOT NULL,
    CONSTRAINT Invoice_pk PRIMARY KEY  (invoice_id)
);

CREATE TABLE Item (
    item_id int  NOT NULL IDENTITY(1, 1),
    item_name varchar(250)  NOT NULL,
    item_description varchar(500)  NOT NULL,
    part_number varchar(150)  NOT NULL,
    CONSTRAINT Item_pk PRIMARY KEY  (item_id)
);

CREATE TABLE Item_owner (
    id_user int  NOT NULL,
    invoice_id int  NOT NULL,
    qty int  NOT NULL,
    owned_item_id int  NOT NULL,
    CONSTRAINT Item_owner_pk PRIMARY KEY  (id_user,invoice_id)
);

CREATE TABLE Log_Type (
    log_type_id int  NOT NULL IDENTITY(1, 1),
    log_type_name varchar(50)  NOT NULL,
    CONSTRAINT Log_Type_pk PRIMARY KEY  (log_type_id)
);

CREATE TABLE Note (
    note_id int  NOT NULL IDENTITY(1, 1),
    note_description varchar(500)  NOT NULL,
    note_date datetime  NOT NULL,
    users_id int  NOT NULL,
    CONSTRAINT Note_pk PRIMARY KEY  (note_id)
);

CREATE TABLE Notes_Delivery (
    delivery_id int  NOT NULL,
    note_id int  NOT NULL,
    CONSTRAINT Notes_Delivery_pk PRIMARY KEY  (delivery_id,note_id)
);

CREATE TABLE Object_type (
    object_type_id int  NOT NULL IDENTITY(1, 1),
    object_type_name varchar(30)  NOT NULL,
    CONSTRAINT Object_type_pk PRIMARY KEY  (object_type_id)
);

CREATE TABLE Offer (
    offer_id int  NOT NULL IDENTITY(1, 1),
    offer_name varchar(100)  NOT NULL,
    creation_date int  NOT NULL,
    modification_date int  NOT NULL,
    organizations_id int  NULL,
    path_to_file varchar(200)  NOT NULL,
    offer_status_id int  NOT NULL,
    CONSTRAINT Offer_pk PRIMARY KEY  (offer_id)
);

CREATE TABLE Offer_Item (
    offer_id int  NOT NULL,
    id_user int  NOT NULL,
    invoice_id int  NOT NULL,
    qty int  NOT NULL,
    selling_price decimal(20,2)  NOT NULL,
    CONSTRAINT Offer_Item_pk PRIMARY KEY  (offer_id,id_user,invoice_id)
);

CREATE TABLE Offer_status (
    offer_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(30)  NOT NULL,
    CONSTRAINT Offer_status_pk PRIMARY KEY  (offer_id)
);

CREATE TABLE Org_User (
    org_user_id int  NOT NULL IDENTITY(1, 1),
    role_id int  NOT NULL,
    organizations_id int  NOT NULL,
    CONSTRAINT Org_User_pk PRIMARY KEY  (org_user_id)
);

CREATE TABLE Organization (
    organization_id int  NOT NULL IDENTITY(1, 1),
    org_name varchar(50)  NOT NULL,
    nip int  NULL,
    street varchar(200)  NOT NULL,
    city varchar(200)  NOT NULL,
    postal_code varchar(25)  NOT NULL,
    credit_limit int  NULL,
    country_id int  NOT NULL,
    CONSTRAINT Organization_pk PRIMARY KEY  (organization_id)
);

CREATE TABLE Outside_Item (
    item_id int  NOT NULL,
    organization_id int  NOT NULL,
    purchase_price decimal(20,2)  NOT NULL,
    qty int  NOT NULL,
    CONSTRAINT Outside_Item_pk PRIMARY KEY  (item_id,organization_id)
);

CREATE TABLE Outside_Item_Offer (
    offer_id int  NOT NULL,
    outside_item_id int  NOT NULL,
    organization_id int  NOT NULL,
    qty int  NOT NULL,
    selling_price decimal(20,2)  NOT NULL,
    CONSTRAINT Outside_Item_Offer_pk PRIMARY KEY  (offer_id,organization_id,outside_item_id)
);

CREATE TABLE Outside_item_owner (
    outside_item_id int  NOT NULL,
    organization_id int  NOT NULL,
    users_id int  NOT NULL,
    CONSTRAINT Outside_item_owner_pk PRIMARY KEY  (outside_item_id,users_id,organization_id)
);

CREATE TABLE Owned_Item (
    invoice_id int  NOT NULL,
    owned_item_id int  NOT NULL,
    qty int  NOT NULL,
    CONSTRAINT Owned_Item_pk PRIMARY KEY  (owned_item_id,invoice_id)
);

CREATE TABLE Payment_Method (
    payment_method_id int  NOT NULL IDENTITY(1, 1),
    method_name varchar(35)  NOT NULL,
    CONSTRAINT Payment_Method_pk PRIMARY KEY  (payment_method_id)
);

CREATE TABLE Payment_Status (
    payment_status_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(30)  NOT NULL,
    CONSTRAINT Payment_Status_pk PRIMARY KEY  (payment_status_id)
);

CREATE TABLE Proforma (
    proforma_id int  NOT NULL IDENTITY(1, 1),
    proforma_number varchar(40)  NOT NULL,
    seller int  NOT NULL,
    buyer int  NOT NULL,
    proforma_date datetime  NOT NULL,
    transport_cost decimal(6,2)  NOT NULL,
    note varchar(500)  NOT NULL,
    in_system bit  NOT NULL,
    proforma_file_path varchar(500)  NULL,
    taxes int  NOT NULL,
    payment_method_id int  NOT NULL,
    currency_value_date datetime  NOT NULL,
    currency_name varchar(5)  NOT NULL,
    invoice_id int  NULL,
    CONSTRAINT Proforma_pk PRIMARY KEY  (proforma_id)
);

CREATE TABLE Proforma_Owned_Item (
    proforma_id int  NOT NULL,
    owned_item_id int  NOT NULL,
    invoice_id int  NOT NULL,
    qty int  NOT NULL,
    selling_price decimal(20,2)  NOT NULL,
    CONSTRAINT Proforma_Owned_Item_pk PRIMARY KEY  (owned_item_id,invoice_id,proforma_id)
);

CREATE TABLE Purchase_Price (
    purchase_price_id int  NOT NULL IDENTITY(1, 1),
    owned_item_id int  NOT NULL,
    invoice_id int  NOT NULL,
    price_date int  NOT NULL,
    curenncy varchar(5)  NOT NULL,
    purchase_price decimal(20,2)  NOT NULL,
    CONSTRAINT Purchase_Price_pk PRIMARY KEY  (purchase_price_id,owned_item_id,invoice_id)
);

CREATE TABLE Request (
    request_id int  NOT NULL IDENTITY(1, 1),
    id_user int  NOT NULL,
    request_status_id int  NOT NULL,
    CONSTRAINT Request_pk PRIMARY KEY  (request_id)
);

CREATE TABLE Request_Credit_note (
    credit_note_id int  NOT NULL,
    request_id int  NOT NULL,
    CONSTRAINT Request_Credit_note_pk PRIMARY KEY  (credit_note_id)
);

CREATE TABLE Request_Invoice (
    request_id int  NOT NULL,
    invoice_id int  NOT NULL,
    CONSTRAINT Request_Invoice_pk PRIMARY KEY  (request_id,invoice_id)
);

CREATE TABLE Request_Note (
    request_id int  NOT NULL,
    note_id int  NOT NULL,
    CONSTRAINT Request_Note_pk PRIMARY KEY  (request_id,note_id)
);

CREATE TABLE Request_Proforma (
    request_id int  NOT NULL,
    proforma_id int  NOT NULL,
    CONSTRAINT Request_Proforma_pk PRIMARY KEY  (request_id,proforma_id)
);

CREATE TABLE Request_status (
    request_status_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(50)  NOT NULL,
    CONSTRAINT Request_status_pk PRIMARY KEY  (request_status_id)
);

CREATE TABLE Solo_User (
    solo_user_id int  NOT NULL IDENTITY(1, 1),
    organizations_id int  NOT NULL,
    CONSTRAINT Solo_User_pk PRIMARY KEY  (solo_user_id)
);

CREATE TABLE Taxes (
    taxes_id int  NOT NULL IDENTITY(1, 1),
    tax_value decimal(5,2)  NOT NULL,
    CONSTRAINT Taxes_pk PRIMARY KEY  (taxes_id)
);

CREATE TABLE User_notification (
    notification_id int  NOT NULL IDENTITY(1, 1),
    users_id int  NOT NULL,
    info varchar(250)  NOT NULL,
    object_type_id int  NOT NULL,
    CONSTRAINT User_notification_pk PRIMARY KEY  (notification_id)
);

CREATE TABLE User_role (
    role_id int  NOT NULL IDENTITY(1, 1),
    role_name varchar(25)  NOT NULL,
    CONSTRAINT User_role_pk PRIMARY KEY  (role_id)
);

CREATE TABLE Waybill (
    waybill_id int  NOT NULL IDENTITY(1, 1),
    waybill varchar(50)  NOT NULL,
    deliveries_id int  NOT NULL,
    delivery_company_id int  NOT NULL,
    CONSTRAINT Waybill_pk PRIMARY KEY  (waybill_id)
);

ALTER TABLE Availability_Status_Organization ADD CONSTRAINT Availability_Status_Organization_Availability_Status_relation
    FOREIGN KEY (availability_status_id)
    REFERENCES Availability_Status (availability_status_id);

ALTER TABLE Availability_Status_Organization ADD CONSTRAINT Availability_Status_Organization_Organization_relation
    FOREIGN KEY (organization_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Credit_note ADD CONSTRAINT Credit_note_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Credit_note_Items ADD CONSTRAINT Credit_note_Items_Credit_note_relation
    FOREIGN KEY (credit_note_id)
    REFERENCES Credit_note (id_credit_note);

ALTER TABLE Credit_note_Items ADD CONSTRAINT Credit_note_Items_Owned_Item_relation
    FOREIGN KEY (owned_item_id,invoice_id)
    REFERENCES Owned_Item (owned_item_id,invoice_id);

ALTER TABLE Currency_Value ADD CONSTRAINT Currency_Value_Currency_Name_relation
    FOREIGN KEY (currency_name)
    REFERENCES Currency_Name (curenncy);

ALTER TABLE Currency_Value_Offer ADD CONSTRAINT Currency_Value_Offer_Currency_Value_relation
    FOREIGN KEY (curency_date,currancy_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Currency_Value_Offer ADD CONSTRAINT Currency_Value_Offer_Offer_relation
    FOREIGN KEY (offer_id)
    REFERENCES Offer (offer_id);

ALTER TABLE Delivery ADD CONSTRAINT Delivery_Delivery_Status_relation
    FOREIGN KEY (delivery_status_id)
    REFERENCES Delivery_Status (delivery_status_id);

ALTER TABLE Delivery ADD CONSTRAINT Delivery_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Delivery ADD CONSTRAINT Delivery_Proforma_relation
    FOREIGN KEY (proforma_id)
    REFERENCES Proforma (proforma_id);

ALTER TABLE EAN ADD CONSTRAINT EAN_Item_relation
    FOREIGN KEY (item_id)
    REFERENCES Item (item_id);

ALTER TABLE Invoice ADD CONSTRAINT Invoice_Currency_Value_relation
    FOREIGN KEY (currency_value_date,currency_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Invoice ADD CONSTRAINT Invoice_Payment_Method_relation
    FOREIGN KEY (payment_method_id)
    REFERENCES Payment_Method (payment_method_id);

ALTER TABLE Invoice ADD CONSTRAINT Invoice_Payments_Status_relation
    FOREIGN KEY (payments_status_Id)
    REFERENCES Payment_Status (payment_status_id);

ALTER TABLE Invoice ADD CONSTRAINT Invoice_Taxes_relation
    FOREIGN KEY (taxes)
    REFERENCES Taxes (taxes_id);

ALTER TABLE Item_owner ADD CONSTRAINT Item_owner_Owned_Item_relation
    FOREIGN KEY (owned_item_id,invoice_id)
    REFERENCES Owned_Item (owned_item_id,invoice_id);

ALTER TABLE Item_owner ADD CONSTRAINT Item_owner_User_relation
    FOREIGN KEY (id_user)
    REFERENCES App_User (id_user);

ALTER TABLE Action_Log ADD CONSTRAINT Log_Log_Type_relation
    FOREIGN KEY (log_type_id)
    REFERENCES Log_Type (log_type_id);

ALTER TABLE Action_Log ADD CONSTRAINT Log_User_relation
    FOREIGN KEY (users_id)
    REFERENCES App_User (id_user);

ALTER TABLE Notes_Delivery ADD CONSTRAINT Note_Delivery_Delivery_relation
    FOREIGN KEY (delivery_id)
    REFERENCES Delivery (delivery_id);

ALTER TABLE Notes_Delivery ADD CONSTRAINT Note_Delivery_Note_relation
    FOREIGN KEY (note_id)
    REFERENCES Note (note_id);

ALTER TABLE Note ADD CONSTRAINT Note_User_relation
    FOREIGN KEY (users_id)
    REFERENCES App_User (id_user);

ALTER TABLE User_notification ADD CONSTRAINT Notification_Object_type_relation
    FOREIGN KEY (object_type_id)
    REFERENCES Object_type (object_type_id);

ALTER TABLE User_notification ADD CONSTRAINT Notification_User_relation
    FOREIGN KEY (users_id)
    REFERENCES App_User (id_user);

ALTER TABLE Offer_Item ADD CONSTRAINT Offer_Item_Item_owner_relation
    FOREIGN KEY (id_user,invoice_id)
    REFERENCES Item_owner (id_user,invoice_id);

ALTER TABLE Offer_Item ADD CONSTRAINT Offer_Items_relation
    FOREIGN KEY (offer_id)
    REFERENCES Offer (offer_id);

ALTER TABLE Offer ADD CONSTRAINT Offer_Offer_status_relation
    FOREIGN KEY (offer_status_id)
    REFERENCES Offer_status (offer_id);

ALTER TABLE Offer ADD CONSTRAINT Offer_Organization_relation
    FOREIGN KEY (organizations_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Org_User ADD CONSTRAINT Org_User_Organization_relation
    FOREIGN KEY (organizations_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Org_User ADD CONSTRAINT Org_User_Role_relation
    FOREIGN KEY (role_id)
    REFERENCES User_role (role_id);

ALTER TABLE Organization ADD CONSTRAINT Organization_Country_relation
    FOREIGN KEY (country_id)
    REFERENCES Country (country_id);

ALTER TABLE Outside_Item ADD CONSTRAINT Outside_Item_Item_relation
    FOREIGN KEY (item_id)
    REFERENCES Item (item_id);

ALTER TABLE Outside_Item_Offer ADD CONSTRAINT Outside_Item_Offer_Offer_relation
    FOREIGN KEY (offer_id)
    REFERENCES Offer (offer_id);

ALTER TABLE Outside_Item_Offer ADD CONSTRAINT Outside_Item_Offer_Outside_Item_relation
    FOREIGN KEY (outside_item_id,organization_id)
    REFERENCES Outside_Item (item_id,organization_id);

ALTER TABLE Outside_Item ADD CONSTRAINT Outside_Item_Organization_relation
    FOREIGN KEY (organization_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Outside_item_owner ADD CONSTRAINT Outside_item_owner_Outside_Item_relation
    FOREIGN KEY (outside_item_id,organization_id)
    REFERENCES Outside_Item (item_id,organization_id);

ALTER TABLE Outside_item_owner ADD CONSTRAINT Outside_item_owner_User_relation
    FOREIGN KEY (users_id)
    REFERENCES App_User (id_user);

ALTER TABLE Owned_Item ADD CONSTRAINT Owned_Item_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Owned_Item ADD CONSTRAINT Owned_Item_Item_relation
    FOREIGN KEY (owned_item_id)
    REFERENCES Item (item_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Currency_Value
    FOREIGN KEY (currency_value_date,currency_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Proforma_Owned_Item ADD CONSTRAINT Proforma_Owned_Item_Owned_Item_relation
    FOREIGN KEY (owned_item_id,invoice_id)
    REFERENCES Owned_Item (owned_item_id,invoice_id);

ALTER TABLE Proforma_Owned_Item ADD CONSTRAINT Proforma_Owned_Item_Proforma_relation
    FOREIGN KEY (proforma_id)
    REFERENCES Proforma (proforma_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Payment_Method_relation
    FOREIGN KEY (payment_method_id)
    REFERENCES Payment_Method (payment_method_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Taxes_relation
    FOREIGN KEY (taxes)
    REFERENCES Taxes (taxes_id);

ALTER TABLE Purchase_Price ADD CONSTRAINT Purchase_Currency_Name_relation
    FOREIGN KEY (curenncy)
    REFERENCES Currency_Name (curenncy);

ALTER TABLE Purchase_Price ADD CONSTRAINT Purchase_Price_Owned_Item_relation
    FOREIGN KEY (owned_item_id,invoice_id)
    REFERENCES Owned_Item (owned_item_id,invoice_id);

ALTER TABLE Request_Credit_note ADD CONSTRAINT Request_Credit_note_Credit_note_relation
    FOREIGN KEY (credit_note_id)
    REFERENCES Credit_note (id_credit_note);

ALTER TABLE Request_Invoice ADD CONSTRAINT Request_Invoice_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Request_Invoice ADD CONSTRAINT Request_Invoice_Request_relation
    FOREIGN KEY (request_id)
    REFERENCES Request (request_id);

ALTER TABLE Request_Note ADD CONSTRAINT Request_Note_Note_relation
    FOREIGN KEY (note_id)
    REFERENCES Note (note_id);

ALTER TABLE Request_Note ADD CONSTRAINT Request_Note_Request_relation
    FOREIGN KEY (request_id)
    REFERENCES Request (request_id);

ALTER TABLE Request ADD CONSTRAINT Request_Org_User_relation
    FOREIGN KEY (id_user)
    REFERENCES Org_User (org_user_id);

ALTER TABLE Request_Proforma ADD CONSTRAINT Request_Proforma_Proforma_relation
    FOREIGN KEY (proforma_id)
    REFERENCES Proforma (proforma_id);

ALTER TABLE Request_Proforma ADD CONSTRAINT Request_Proforma_Request_relation
    FOREIGN KEY (request_id)
    REFERENCES Request (request_id);

ALTER TABLE Request_Credit_note ADD CONSTRAINT Request_Request_Credit_note_relation
    FOREIGN KEY (request_id)
    REFERENCES Request (request_id);

ALTER TABLE Request ADD CONSTRAINT Request_Request_status_relation
    FOREIGN KEY (request_status_id)
    REFERENCES Request_status (request_status_id);

ALTER TABLE Invoice ADD CONSTRAINT Seller_Organization_relation
    FOREIGN KEY (seller)
    REFERENCES Organization (organization_id);

ALTER TABLE Proforma ADD CONSTRAINT Seller_Proforma_relation
    FOREIGN KEY (seller)
    REFERENCES Organization (organization_id);

ALTER TABLE Solo_User ADD CONSTRAINT Solo_User_Organization_relation
    FOREIGN KEY (organizations_id)
    REFERENCES Organization (organization_id);

ALTER TABLE App_User ADD CONSTRAINT User_Org_User_relation
    FOREIGN KEY (org_user_id)
    REFERENCES Org_User (org_user_id);

ALTER TABLE App_User ADD CONSTRAINT User_Solo_User_relation
    FOREIGN KEY (solo_user_id)
    REFERENCES Solo_User (solo_user_id);

ALTER TABLE Waybill ADD CONSTRAINT Waybill_Delivery_company_relation
    FOREIGN KEY (delivery_company_id)
    REFERENCES Delivery_company (delivery_company_id);

ALTER TABLE Waybill ADD CONSTRAINT Waybill_Delivery_relation
    FOREIGN KEY (deliveries_id)
    REFERENCES Delivery (delivery_id);

ALTER TABLE Proforma ADD CONSTRAINT buyer_Proforma_relation
    FOREIGN KEY (buyer)
    REFERENCES Organization (organization_id);

ALTER TABLE Invoice ADD CONSTRAINT buyer_invoice_relation
    FOREIGN KEY (buyer)
    REFERENCES Organization (organization_id);

go

-- Data seed

INSERT INTO User_role(role_name)
Values ('Admin'), ('Accountant'), ('Merchant'), ('Warehouse Manager');

INSERT INTO Taxes (tax_value)
Values (0.00), (5.00), (8.00), (23.00);

INSERT INTO Currency_Name (curenncy)
Values ('EUR'), ('PLN'), ('USD');

INSERT INTO Delivery_Status(status_name)
Values ('Fulfilled'), ('In transport'), ('Delivered with issues'), ('Preparing'), ('Rejected');

INSERT INTO Object_type (object_type_name)
Values ('Proforma'), ('Invoice'), ('Request'), ('Item'), ('Delivery'), ('Role'), ('Client'), ('Pricelist'), ('Credit note');

INSERT INTO Offer_status (status_name)
Values ('Active'), ('Deactivated');

INSERT INTO Payment_Status (status_name)
Values ('Paid'), ('Unpaid'), ('Due to');

INSERT INTO Request_status (status_name)
Values ('Fulfilled'), ('Request cancelled'), ('In progress');

INSERT INTO Country (country_name)
Values ('Afghanistan'),
('Albania'),
('Algeria'),
('Andorra'),
('Angola'),
('Antigua and Barbuda'),
('Argentina'),
('Armenia'),
('Australia'),
('Austria'),
('Azerbaijan'),
('The Bahamas'),
('Bahrain'),
('Bangladesh'),
('Barbados'),
('Belarus'),
('Belgium'),
('Belize'),
('Benin'),
('Bhutan'),
('Bolivia'),
('Bosnia and Herzegovina'),
('Botswana'),
('Brazil'),
('Brunei'),
('Bulgaria'),
('Burkina Faso'),
('Burundi'),
('Cabo Verde'),
('Cambodia'),
('Cameroon'),
('Canada'),
('Central African Republic'),
('Chad'),
('Chile'),
('China'),
('Colombia'),
('Comoros'),
('Congo, Democratic Republic of the'),
('Congo, Republic of the'),
('Costa Rica'),
('C�te d�Ivoire'),
('Croatia'),
('Cuba'),
('Cyprus'),
('Czech Republic'),
('Denmark'),
('Djibouti'),
('Dominica'),
('Dominican Republic'),
('East Timor (Timor-Leste)'),
('Ecuador'),
('Egypt'),
('El Salvador'),
('Equatorial Guinea'),
('Eritrea'),
('Estonia'),
('Eswatini'),
('Ethiopia'),
('Fiji'),
('Finland'),
('France'),
('Gabon'),
('The Gambia'),
('Georgia'),
('Germany'),
('Ghana'),
('Greece'),
('Grenada'),
('Guatemala'),
('Guinea'),
('Guinea-Bissau'),
('Guyana'),
('Haiti'),
('Honduras'),
('Hungary'),
('Iceland'),
('India'),
('Indonesia'),
('Iran'),
('Iraq'),
('Ireland'),
('Israel'),
('Italy'),
('Jamaica'),
('Japan'),
('Jordan'),
('Kazakhstan'),
('Kenya'),
('Kiribati'),
('Korea, North'),
('Korea, South'),
('Kosovo'),
('Kuwait'),
('Kyrgyzstan'),
('Laos'),
('Latvia'),
('Lebanon'),
('Lesotho'),
('Liberia'),
('Libya'),
('Liechtenstein'),
('Lithuania'),
('Luxembourg'),
('Madagascar'),
('Malawi'),
('Malaysia'),
('Maldives'),
('Mali'),
('Malta'),
('Marshall Islands'),
('Mauritania'),
('Mauritius'),
('Mexico'),
('Micronesia, Federated States of'),
('Moldova'),
('Monaco'),
('Mongolia'),
('Montenegro'),
('Morocco'),
('Mozambique'),
('Myanmar (Burma)'),
('Namibia'),
('Nauru'),
('Nepal'),
('Netherlands'),
('New Zealand'),
('Nicaragua'),
('Niger'),
('Nigeria'),
('North Macedonia'),
('Norway'),
('Oman'),
('Pakistan'),
('Palau'),
('Panama'),
('Papua New Guinea'),
('Paraguay'),
('Peru'),
('Philippines'),
('Poland'),
('Portugal'),
('Qatar'),
('Romania'),
('Russia'),
('Rwanda'),
('Saint Kitts and Nevis'),
('Saint Lucia'),
('Saint Vincent and the Grenadines'),
('Samoa'),
('San Marino'),
('Sao Tome and Principe'),
('Saudi Arabia'),
('Senegal'),
('Serbia'),
('Seychelles'),
('Sierra Leone'),
('Singapore'),
('Slovakia'),
('Slovenia'),
('Solomon Islands'),
('Somalia'),
('South Africa'),
('Spain'),
('Sri Lanka'),
('Sudan'),
('Sudan, South'),
('Suriname'),
('Sweden'),
('Switzerland'),
('Syria'),
('Taiwan'),
('Tajikistan'),
('Tanzania'),
('Thailand'),
('Togo'),
('Tonga'),
('Trinidad and Tobago'),
('Tunisia'),
('Turkey'),
('Turkmenistan'),
('Tuvalu'),
('Uganda'),
('Ukraine'),
('United Arab Emirates'),
('United Kingdom'),
('United States'),
('Uruguay'),
('Uzbekistan'),
('Vanuatu'),
('Vatican City'),
('Venezuela'),
('Vietnam'),
('Yemen'),
('Zambia'),
('Zimbabwe');

go

-- Indexes

go

-- Text search

exec Sp_fulltext_database 'enable';

CREATE FULLTEXT CATALOG invoice_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG item_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG offer_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG organization_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG proforma_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

go

-- Add tables to text search

CREATE FULLTEXT INDEX ON [dbo].[Invoice] KEY INDEX [Invoice_pk] ON ([invoice_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[Invoice] ADD ([invoice_number])
ALTER FULLTEXT INDEX ON [dbo].[Invoice] ENABLE

CREATE FULLTEXT INDEX ON [dbo].[Item] KEY INDEX [Item_pk] ON ([item_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[Item] ADD ([item_name])
ALTER FULLTEXT INDEX ON [dbo].[Item] ADD ([part_number])
ALTER FULLTEXT INDEX ON [dbo].[Item] ENABLE

CREATE FULLTEXT INDEX ON [dbo].[Offer] KEY INDEX [Offer_pk] ON ([offer_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[Offer] ADD ([offer_name])
ALTER FULLTEXT INDEX ON [dbo].[Offer] ENABLE

CREATE FULLTEXT INDEX ON [dbo].[Organization] KEY INDEX [Organization_pk] ON ([organization_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[Organization] ADD ([org_name])
ALTER FULLTEXT INDEX ON [dbo].[Organization] ADD ([street])
ALTER FULLTEXT INDEX ON [dbo].[Organization] ADD ([city])
ALTER FULLTEXT INDEX ON [dbo].[Organization] ENABLE

CREATE FULLTEXT INDEX ON [dbo].[Proforma] KEY INDEX [Proforma_pk] ON ([proforma_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[Proforma] ADD ([proforma_number])
ALTER FULLTEXT INDEX ON [dbo].[Proforma] ENABLE

GO

