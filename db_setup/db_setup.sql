
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

CREATE TABLE Calculated_Credit_note_Price (
    currency_name varchar(5)  NOT NULL,
    update_date datetime  NOT NULL,
    credit_item_id int  NOT NULL,
    price decimal(20,2)  NOT NULL,
    CONSTRAINT Calculated_Credit_note_Price_pk PRIMARY KEY  (currency_name,update_date,credit_item_id)
);

CREATE TABLE Calculated_Price (
    purchase_price_id int  NOT NULL,
    update_date datetime  NOT NULL,
    currency_name varchar(5)  NOT NULL,
    price decimal(20,2)  NOT NULL,
    CONSTRAINT Calculated_Price_pk PRIMARY KEY  (purchase_price_id,update_date,currency_name)
);

CREATE TABLE Country (
    country_id int  NOT NULL IDENTITY(1, 1),
    country_name varchar(70)  NOT NULL,
    CONSTRAINT Country_pk PRIMARY KEY  (country_id)
);

CREATE TABLE Credit_note (
    id_credit_note int  NOT NULL IDENTITY(1, 1),
    credit_note_number varchar(50)  NOT NULL,
    credit_note_date date  NOT NULL,
    in_system bit  NOT NULL,
    is_paid bit  NOT NULL,
    note varchar(500)  NOT NULL,
    invoice_id int  NOT NULL,
    credit_file_path varchar(500)  NULL,
    id_user int NOT NULL,
    CONSTRAINT Credit_note_pk PRIMARY KEY  (id_credit_note)
);

CREATE TABLE Credit_note_Items (
    credit_item_id int  NOT NULL IDENTITY(1, 1),
    credit_note_id int  NOT NULL,
    purchase_price_id int  NOT NULL,
    qty int  NOT NULL,
    new_price decimal(20,2)  NOT NULL,
    CONSTRAINT Credit_note_Items_pk PRIMARY KEY  (credit_item_id)
);

CREATE TABLE Currency_Name (
    currency varchar(5)  NOT NULL,
    CONSTRAINT Currency_Name_pk PRIMARY KEY  (currency)
);

CREATE TABLE Currency_Value (
    currency_name varchar(5)  NOT NULL,
    update_date datetime  NOT NULL,
    currency_value decimal(5,2)  NOT NULL,
    CONSTRAINT Currency_Value_pk PRIMARY KEY  (update_date,currency_name)
);

CREATE TABLE Delivery (
    delivery_id int  NOT NULL IDENTITY(1, 1),
    estimated_delivery_date datetime  NOT NULL,
    delivery_date datetime  NULL,
    delivery_status_id int  NOT NULL,
    proforma_id int  NOT NULL,
    delivery_company_id int  NOT NULL,
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
    ean varchar(50)  NOT NULL,
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
    transport_cost decimal(20,2)  NOT NULL,
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
    owned_item_id int  NOT NULL,
    qty int  NOT NULL,
    CONSTRAINT Item_owner_pk PRIMARY KEY  (id_user,invoice_id,owned_item_id)
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
    creation_date datetime  NOT NULL,
    modification_date datetime  NOT NULL,
    path_to_file varchar(200)  NOT NULL,
    offer_status_id int  NOT NULL,
    max_qty int  NOT NULL,
    currency_name varchar(5)  NOT NULL,
    id_user int  NOT NULL,
    CONSTRAINT Offer_pk PRIMARY KEY  (offer_id)
);

CREATE TABLE Offer_Item (
    offer_id int  NOT NULL,
    item_id int  NOT NULL,
    selling_price decimal(20,2)  NOT NULL,
    CONSTRAINT Offer_Item_pk PRIMARY KEY  (offer_id, item_id)
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
    availability_status_id int  NULL,
    CONSTRAINT Organization_pk PRIMARY KEY  (organization_id)
);

CREATE TABLE Outside_Item (
    item_id int  NOT NULL,
    organization_id int  NOT NULL,
    purchase_price decimal(20,2)  NOT NULL,
    qty int  NOT NULL,
    currency_name varchar(5)  NOT NULL,
    CONSTRAINT Outside_Item_pk PRIMARY KEY  (item_id,organization_id)
);

CREATE TABLE Owned_Item (
    invoice_id int  NOT NULL,
    owned_item_id int  NOT NULL,
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
    id_user int  NOT NULL,
    CONSTRAINT Proforma_pk PRIMARY KEY  (proforma_id)
);

CREATE TABLE Proforma_Future_Item (
    proforma_future_item_id int  NOT NULL IDENTITY(1, 1),
    proforma_id int  NOT NULL,
    item_id int  NOT NULL,
    qty int  NOT NULL,
    purchase_price decimal(20,2)  NOT NULL,
    CONSTRAINT Proforma_Future_Item_pk PRIMARY KEY  (proforma_future_item_id)
);

CREATE TABLE Proforma_Owned_Item (
    proforma_owned_item_id int  NOT NULL IDENTITY(1, 1),
    proforma_id int  NOT NULL,
    purchase_price_id int  NOT NULL,
    qty int  NOT NULL,
    selling_price decimal(20,2)  NOT NULL,
    CONSTRAINT Proforma_Owned_Item_pk PRIMARY KEY  (proforma_owned_item_id)
);

CREATE TABLE Purchase_Price (
    purchase_price_id int  NOT NULL IDENTITY(1, 1),
    qty int  NOT NULL,
    price decimal(20,2)  NOT NULL,
    owned_item_id int  NOT NULL,
    invoice_id int  NOT NULL,
    CONSTRAINT Purchase_Price_pk PRIMARY KEY  (purchase_price_id)
);

CREATE TABLE Request (
    request_id int  NOT NULL IDENTITY(1, 1),
    id_user_creator int  NOT NULL,
    id_user_receiver int  NOT NULL,
    request_status_id int  NOT NULL,
    object_type_id int  NOT NULL,
    filePath varchar(500)  NULL,
    note varchar(500)  NOT NULL,
    title varchar(100)  NOT NULL,
    creation_date datetime  NOT NULL,
    CONSTRAINT Request_pk PRIMARY KEY  (request_id)
);

CREATE TABLE Request_status (
    request_status_id int  NOT NULL IDENTITY(1, 1),
    status_name varchar(50)  NOT NULL,
    CONSTRAINT Request_status_pk PRIMARY KEY  (request_status_id)
);

CREATE TABLE Selling_Price (
    selling_price_id int  NOT NULL IDENTITY(1, 1),
    sell_invoice_id int  NOT NULL,
    purchase_price_id int  NOT NULL,
    id_user int  NOT NULL,
    qty int  NOT NULL,
    price decimal(20,2)  NOT NULL,
    CONSTRAINT Selling_Price_pk PRIMARY KEY  (selling_price_id)
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

CREATE TABLE User_client (
    users_id int  NOT NULL,
    organization_id int  NOT NULL,
    CONSTRAINT User_client_pk PRIMARY KEY  (users_id,organization_id)
);

CREATE TABLE User_notification (
    notification_id int  NOT NULL IDENTITY(1, 1),
    users_id int  NOT NULL,
    info varchar(250)  NOT NULL,
    object_type_id int  NOT NULL,
    reference varchar(300)  NULL,
    is_read bit  NOT NULL,
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
    CONSTRAINT Waybill_pk PRIMARY KEY  (waybill_id)
);

ALTER TABLE Calculated_Credit_note_Price ADD CONSTRAINT Calculated_Credit_note_Price_Credit_note_Items
    FOREIGN KEY (credit_item_id)
    REFERENCES Credit_note_Items (credit_item_id);

ALTER TABLE Calculated_Credit_note_Price ADD CONSTRAINT Calculated_Credit_note_Price_Currency_Value
    FOREIGN KEY (update_date,currency_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Calculated_Price ADD CONSTRAINT Calculated_Price_Currency_Value_relation
    FOREIGN KEY (update_date,currency_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Calculated_Price ADD CONSTRAINT Calculated_Price_Purchase_Price_relation
    FOREIGN KEY (purchase_price_id)
    REFERENCES Purchase_Price (purchase_price_id);

ALTER TABLE User_client ADD CONSTRAINT Client_User_relation
    FOREIGN KEY (organization_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Credit_note ADD CONSTRAINT Credit_note_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Credit_note ADD CONSTRAINT Credit_note_App_User_relation
    FOREIGN KEY (id_user)
    REFERENCES App_User (id_user);

ALTER TABLE Credit_note_Items ADD CONSTRAINT Credit_note_Items_Credit_note_relation
    FOREIGN KEY (credit_note_id)
    REFERENCES Credit_note (id_credit_note);

ALTER TABLE Credit_note_Items ADD CONSTRAINT Credit_note_Items_Purchase_Price_relation
    FOREIGN KEY (purchase_price_id)
    REFERENCES Purchase_Price (purchase_price_id);

ALTER TABLE Currency_Value ADD CONSTRAINT Currency_Value_Currency_Name_relation
    FOREIGN KEY (currency_name)
    REFERENCES Currency_Name (currency);

ALTER TABLE Delivery ADD CONSTRAINT Delivery_Delivery_company_relation
    FOREIGN KEY (delivery_company_id)
    REFERENCES Delivery_company (delivery_company_id);

ALTER TABLE Delivery ADD CONSTRAINT Delivery_Delivery_Status_relation
    FOREIGN KEY (delivery_status_id)
    REFERENCES Delivery_Status (delivery_status_id);

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

ALTER TABLE Offer_Item ADD CONSTRAINT Offer_Item_Item_relation
    FOREIGN KEY (item_id)
    REFERENCES Item (item_id);

ALTER TABLE Offer_Item ADD CONSTRAINT Offer_Items_relation
    FOREIGN KEY (offer_id)
    REFERENCES Offer (offer_id);

ALTER TABLE Offer ADD CONSTRAINT Offer_App_User_relation
    FOREIGN KEY (id_user)
    REFERENCES App_User (id_user);

ALTER TABLE Offer ADD CONSTRAINT Offer_Offer_status_relation
    FOREIGN KEY (offer_status_id)
    REFERENCES Offer_status (offer_id);

ALTER TABLE Offer ADD CONSTRAINT Offer_Currency_Name_relation
    FOREIGN KEY (currency_name)
    REFERENCES Currency_Name (currency);

ALTER TABLE Org_User ADD CONSTRAINT Org_User_Organization_relation
    FOREIGN KEY (organizations_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Org_User ADD CONSTRAINT Org_User_Role_relation
    FOREIGN KEY (role_id)
    REFERENCES User_role (role_id);

ALTER TABLE Organization ADD CONSTRAINT Organization_Availability_Status_relation
    FOREIGN KEY (availability_status_id)
    REFERENCES Availability_Status (availability_status_id);

ALTER TABLE Organization ADD CONSTRAINT Organization_Country_relation
    FOREIGN KEY (country_id)
    REFERENCES Country (country_id);

ALTER TABLE Outside_Item ADD CONSTRAINT Outside_Item_Currency_relation
    FOREIGN KEY (currency_name)
    REFERENCES Currency_Name (currency);

ALTER TABLE Outside_Item ADD CONSTRAINT Outside_Item_Item_relation
    FOREIGN KEY (item_id)
    REFERENCES Item (item_id);

ALTER TABLE Outside_Item ADD CONSTRAINT Outside_Item_Organization_relation
    FOREIGN KEY (organization_id)
    REFERENCES Organization (organization_id);

ALTER TABLE Owned_Item ADD CONSTRAINT Owned_Item_Invoice_relation
    FOREIGN KEY (invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Owned_Item ADD CONSTRAINT Owned_Item_Item_relation
    FOREIGN KEY (owned_item_id)
    REFERENCES Item (item_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Currency_Value
    FOREIGN KEY (currency_value_date,currency_name)
    REFERENCES Currency_Value (update_date,currency_name);

ALTER TABLE Proforma_Future_Item ADD CONSTRAINT Proforma_Future_Item_Item
    FOREIGN KEY (item_id)
    REFERENCES Item (item_id);

ALTER TABLE Proforma_Future_Item ADD CONSTRAINT Proforma_Future_Item_Proforma
    FOREIGN KEY (proforma_id)
    REFERENCES Proforma (proforma_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_App_User_relation
    FOREIGN KEY (id_user)
    REFERENCES App_User (id_user);

ALTER TABLE Proforma_Owned_Item ADD CONSTRAINT Proforma_Owned_Item_Purchase_Price_relation
    FOREIGN KEY (purchase_price_id)
    REFERENCES Purchase_Price (purchase_price_id);

ALTER TABLE Proforma_Owned_Item ADD CONSTRAINT Proforma_Owned_Item_Proforma_relation
    FOREIGN KEY (proforma_id)
    REFERENCES Proforma (proforma_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Payment_Method_relation
    FOREIGN KEY (payment_method_id)
    REFERENCES Payment_Method (payment_method_id);

ALTER TABLE Proforma ADD CONSTRAINT Proforma_Taxes_relation
    FOREIGN KEY (taxes)
    REFERENCES Taxes (taxes_id);

ALTER TABLE Purchase_Price ADD CONSTRAINT Purchase_Price_Owned_Item_relation
    FOREIGN KEY (owned_item_id,invoice_id)
    REFERENCES Owned_Item (owned_item_id,invoice_id);

ALTER TABLE Request ADD CONSTRAINT Request_Request_status_relation
    FOREIGN KEY (request_status_id)
    REFERENCES Request_status (request_status_id);

ALTER TABLE Invoice ADD CONSTRAINT Seller_Organization_relation
    FOREIGN KEY (seller)
    REFERENCES Organization (organization_id);

ALTER TABLE Proforma ADD CONSTRAINT Seller_Proforma_relation
    FOREIGN KEY (seller)
    REFERENCES Organization (organization_id);

ALTER TABLE Selling_Price ADD CONSTRAINT Selling_Price_Invoice
    FOREIGN KEY (sell_invoice_id)
    REFERENCES Invoice (invoice_id);

ALTER TABLE Selling_Price ADD CONSTRAINT Selling_Price_Purchase_Price_relation
    FOREIGN KEY (purchase_price_id)
    REFERENCES Purchase_Price (purchase_price_id);

ALTER TABLE Solo_User ADD CONSTRAINT Solo_User_Organization_relation
    FOREIGN KEY (organizations_id)
    REFERENCES Organization (organization_id);

ALTER TABLE User_client ADD CONSTRAINT User_Client_relation
    FOREIGN KEY (users_id)
    REFERENCES App_User (id_user);

ALTER TABLE App_User ADD CONSTRAINT User_Org_User_relation
    FOREIGN KEY (org_user_id)
    REFERENCES Org_User (org_user_id);

ALTER TABLE App_User ADD CONSTRAINT User_Solo_User_relation
    FOREIGN KEY (solo_user_id)
    REFERENCES Solo_User (solo_user_id);

ALTER TABLE Waybill ADD CONSTRAINT Waybill_Delivery_relation
    FOREIGN KEY (deliveries_id)
    REFERENCES Delivery (delivery_id);

ALTER TABLE Proforma ADD CONSTRAINT buyer_Proforma_relation
    FOREIGN KEY (buyer)
    REFERENCES Organization (organization_id);

ALTER TABLE Invoice ADD CONSTRAINT buyer_invoice_relation
    FOREIGN KEY (buyer)
    REFERENCES Organization (organization_id);

ALTER TABLE Selling_Price ADD CONSTRAINT Selling_Price_App_User_relation
    FOREIGN KEY (id_user)
    REFERENCES App_User (id_user);

ALTER TABLE Request ADD CONSTRAINT Request_App_User_creator_relation
    FOREIGN KEY (id_user_creator)
    REFERENCES App_User (id_user);

ALTER TABLE Request ADD CONSTRAINT Request_App_User_relation
    FOREIGN KEY (id_user_receiver)
    REFERENCES App_User (id_user);

ALTER TABLE Request ADD CONSTRAINT Request_Object_type_relation
    FOREIGN KEY (object_type_id)
    REFERENCES Object_type (object_type_id);

-- Data seed

INSERT INTO Availability_Status(status_name, days_for_realization)
Values ('2-3 days', 3), ('2 days', 2), ('1 day', 1), ('4 day', 4);

INSERT INTO Log_Type(log_type_name)
Values ('Create'), ('Modify'), ('Delete');

INSERT INTO User_role(role_name)
Values ('Admin'), ('Accountant'), ('Merchant'), ('Warehouse Manager');

INSERT INTO Taxes (tax_value)
Values (0.00), (5.00), (8.00), (23.00);

INSERT INTO Currency_Name (currency)
Values ('EUR'), ('PLN'), ('USD');

INSERT INTO Currency_Value (currency_name, update_date, currency_value)
Values ('PLN','2024-09-03 00:00:00.000',1)

INSERT INTO Delivery_Status(status_name)
Values ('Fulfilled'), ('In transport'), ('Delivered with issues'), ('Preparing'), ('Rejected');

INSERT INTO Object_type (object_type_name)
Values ('User'),('Yours Proformas'), ('Clients Proformas'),('Sales invoices'), ('Yours invoices'), ('Requests'), ('Item'), ('To client delivery'), ('To user delivery'), ('Role'), ('Client'), ('Pricelist'), ('Yours credit notes'), ('Client credit notes');

INSERT INTO Offer_status (status_name)
Values ('Active'), ('Deactivated');

INSERT INTO Payment_Status (status_name)
Values ('Paid'), ('Unpaid'), ('Due to');

INSERT INTO Payment_Method (method_name)
Values ('Cash'), ('Credit card'), ('Transfer'), ('Blik');

INSERT INTO Request_status (status_name)
Values ('Fulfilled'), ('Request cancelled'), ('In progress');

INSERT INTO Country (country_name)
Values ('Afghanistan'),
('Albania'),
('Algeria'),
('Andorra'),
('Anla'),
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
('Bosnia and Herzevina'),
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
('Con, Democratic Republic of the'),
('Con, Republic of the'),
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
('Monlia'),
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
('To'),
('Tonga'),
('Trinidad and Toba'),
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



-- Indexes

CREATE NONCLUSTERED INDEX [App_User_idUser_index] ON [dbo].[App_User]([id_user]);
CREATE NONCLUSTERED INDEX [App_User_email_index] ON [dbo].[App_User]([email]);
CREATE NONCLUSTERED INDEX [App_User_solo_user_id_index] ON [dbo].[App_User]([solo_user_id]);
CREATE NONCLUSTERED INDEX [App_User_org_user_id_index] ON [dbo].[App_User]([org_user_id]);

CREATE NONCLUSTERED INDEX [Waybill_waybill_id_index] ON [dbo].[Waybill]([waybill_id]);
CREATE NONCLUSTERED INDEX [Waybill_deliveries_id_index] ON [dbo].[Waybill]([deliveries_id]);

CREATE NONCLUSTERED INDEX [Delivery_delivery_id_index] ON [dbo].[Delivery]([delivery_id]);
CREATE NONCLUSTERED INDEX [Delivery_delivery_status_id_index] ON [dbo].[Delivery]([delivery_status_id]);
CREATE NONCLUSTERED INDEX [Delivery_proforma_id_index] ON [dbo].[Delivery]([proforma_id]);
CREATE NONCLUSTERED INDEX [Delivery_delivery_company_id_index] ON [dbo].[Delivery]([delivery_company_id]);

CREATE NONCLUSTERED INDEX [Proforma_Owned_Item_proforma_owned_item_id_index] ON [dbo].[Proforma_Owned_Item]([proforma_owned_item_id]);
CREATE NONCLUSTERED INDEX [Proforma_Owned_Item_proforma_id_index] ON [dbo].[Proforma_Owned_Item]([proforma_id]);
CREATE NONCLUSTERED INDEX [Proforma_Owned_Item_purchase_price_id_index] ON [dbo].[Proforma_Owned_Item]([purchase_price_id]);

CREATE NONCLUSTERED INDEX [Proforma_Future_Item_proforma_future_item_id_index] ON [dbo].[Proforma_Future_Item]([proforma_future_item_id]);
CREATE NONCLUSTERED INDEX [Proforma_Future_Item_proforma_id_index] ON [dbo].[Proforma_Future_Item]([proforma_id]);
CREATE NONCLUSTERED INDEX [Proforma_Future_Item_item_id_index] ON [dbo].[Proforma_Future_Item]([item_id]);

CREATE NONCLUSTERED INDEX [Proforma_proforma_id_index] ON [dbo].[Proforma](proforma_id);
CREATE NONCLUSTERED INDEX [Proforma_id_user_index] ON [dbo].[Proforma](id_user);
CREATE NONCLUSTERED INDEX [Proforma_seller_index] ON [dbo].[Proforma](seller);
CREATE NONCLUSTERED INDEX [Proforma_buyer_index] ON [dbo].[Proforma](buyer);
CREATE NONCLUSTERED INDEX [Proforma_payment_method_id_index] ON [dbo].[Proforma](payment_method_id);
CREATE NONCLUSTERED INDEX [Proforma_taxes_index] ON [dbo].[Proforma](taxes);
CREATE NONCLUSTERED INDEX [Proforma_currency_value_date_index] ON [dbo].[Proforma](currency_value_date);
CREATE NONCLUSTERED INDEX [Proforma_currency_name_index] ON [dbo].[Proforma](currency_name);

CREATE NONCLUSTERED INDEX [Credit_note_id_credit_note_index] ON [dbo].[Credit_note](id_credit_note);
CREATE NONCLUSTERED INDEX [Credit_note_invoice_id_index] ON [dbo].[Credit_note](invoice_id);
CREATE NONCLUSTERED INDEX [Credit_note_id_user_index] ON [dbo].[Credit_note](id_user);

CREATE NONCLUSTERED INDEX [Credit_note_Items_credit_item_id_index] ON [dbo].[Credit_note_Items](credit_item_id);
CREATE NONCLUSTERED INDEX [Credit_note_Items_credit_note_id_index] ON [dbo].[Credit_note_Items](credit_note_id);
CREATE NONCLUSTERED INDEX [Credit_note_Items_purchase_price_id_index] ON [dbo].[Credit_note_Items](purchase_price_id);

CREATE NONCLUSTERED INDEX [Invoice_invoice_id_index] ON [dbo].[Invoice](invoice_id);
CREATE NONCLUSTERED INDEX [Invoice_seller_index] ON [dbo].[Invoice](seller);
CREATE NONCLUSTERED INDEX [Invoice_buyer_index] ON [dbo].[Invoice](buyer);
CREATE NONCLUSTERED INDEX [Invoice_taxes_index] ON [dbo].[Invoice](taxes);
CREATE NONCLUSTERED INDEX [Invoice_currency_value_date_index] ON [dbo].[Invoice](currency_value_date);
CREATE NONCLUSTERED INDEX [Invoice_currency_name_index] ON [dbo].[Invoice](currency_name);
CREATE NONCLUSTERED INDEX [Invoice_payment_method_id_index] ON [dbo].[Invoice](payment_method_id);
CREATE NONCLUSTERED INDEX [Invoice_payments_status_Id_index] ON [dbo].[Invoice](payments_status_Id);

CREATE NONCLUSTERED INDEX [Selling_Price_selling_price_id_index] ON [dbo].[Selling_Price](selling_price_id);
CREATE NONCLUSTERED INDEX [Selling_Price_sell_invoice_id_index] ON [dbo].[Selling_Price](sell_invoice_id);
CREATE NONCLUSTERED INDEX [Selling_Price_purchase_price_id_index] ON [dbo].[Selling_Price](purchase_price_id);
CREATE NONCLUSTERED INDEX [Selling_Price_id_user_index] ON [dbo].[Selling_Price](id_user);

CREATE NONCLUSTERED INDEX [Purchase_Price_purchase_price_id_index] ON [dbo].[Purchase_Price](purchase_price_id);
CREATE NONCLUSTERED INDEX [Purchase_Price_owned_item_id_index] ON [dbo].[Purchase_Price](owned_item_id);
CREATE NONCLUSTERED INDEX [Purchase_Price_invoice_id_index] ON [dbo].[Purchase_Price](invoice_id);

CREATE NONCLUSTERED INDEX [Owned_Item_invoice_id_index] ON [dbo].[Owned_Item](invoice_id);
CREATE NONCLUSTERED INDEX [Owned_Item_owned_item_id_index] ON [dbo].[Owned_Item](owned_item_id);
CREATE NONCLUSTERED INDEX [Owned_Item_index] ON [dbo].[Owned_Item](owned_item_id, invoice_id);

CREATE NONCLUSTERED INDEX [Item_owner_id_user_index] ON [dbo].Item_owner(id_user);
CREATE NONCLUSTERED INDEX [Item_owner_invoice_id_index] ON [dbo].Item_owner(invoice_id);
CREATE NONCLUSTERED INDEX [Item_owner_owned_item_id_index] ON [dbo].Item_owner(owned_item_id);
CREATE NONCLUSTERED INDEX [Item_owner_include_index] ON [dbo].Item_owner(id_user, invoice_id, owned_item_id);

CREATE NONCLUSTERED INDEX [User_notification_users_id_index] ON [dbo].[User_notification](users_id);

CREATE NONCLUSTERED INDEX [Organization_organization_id_index] ON [dbo].[Organization](organization_id);
CREATE NONCLUSTERED INDEX [Organization_country_id_index] ON [dbo].[Organization](country_id);
CREATE NONCLUSTERED INDEX [Organization_availability_status_id_index] ON [dbo].[Organization](availability_status_id);

CREATE NONCLUSTERED INDEX [Note_note_id_index] ON [dbo].[Note](note_id);
CREATE NONCLUSTERED INDEX [Note_users_id_index] ON [dbo].[Note](users_id);

CREATE NONCLUSTERED INDEX [Request_request_id_index] ON [dbo].[Request](request_id);
CREATE NONCLUSTERED INDEX [Request_id_user_creator_index] ON [dbo].[Request](id_user_creator);
CREATE NONCLUSTERED INDEX [Request_id_user_receiver_index] ON [dbo].[Request](id_user_receiver);
CREATE NONCLUSTERED INDEX [Request_request_status_id_index] ON [dbo].[Request](request_status_id);
CREATE NONCLUSTERED INDEX [Request_object_type_id_index] ON [dbo].[Request](object_type_id);

CREATE NONCLUSTERED INDEX [Offer_offer_id_index] ON [dbo].[Offer](offer_id);
CREATE NONCLUSTERED INDEX [Offer_offer_status_id_index] ON [dbo].[Offer](offer_status_id);
CREATE NONCLUSTERED INDEX [Offer_currency_name_index] ON [dbo].[Offer](currency_name);
CREATE NONCLUSTERED INDEX [Offer_id_user_index] ON [dbo].[Offer](id_user);

CREATE NONCLUSTERED INDEX [Offer_Item_index] ON [dbo].[Offer_Item](offer_id, item_id);

CREATE NONCLUSTERED INDEX [EAN_ean_index] ON [dbo].[EAN](ean);
CREATE NONCLUSTERED INDEX [EAN_item_id_index] ON [dbo].[EAN](item_id);

CREATE NONCLUSTERED INDEX [Item_item_id_index] ON [dbo].[Item](item_id);

CREATE NONCLUSTERED INDEX [Outside_Item_index] ON [dbo].[Outside_Item](item_id, organization_id);
CREATE NONCLUSTERED INDEX [Outside_Item_currency_name_index] ON [dbo].[Outside_Item](currency_name);
-- Text search

CREATE FULLTEXT CATALOG item_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG offer_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG organization_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG proforma_ft_catalog WITH ACCENT_SENSITIVITY = OFF;

CREATE FULLTEXT CATALOG user_ft_catalog WITH ACCENT_SENSITIVITY = OFF;



-- Add tables to text search

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

CREATE FULLTEXT INDEX ON [dbo].[App_User] KEY INDEX [App_User_pk] ON ([user_ft_catalog]) WITH (CHANGE_TRACKING AUTO)
ALTER FULLTEXT INDEX ON [dbo].[App_User] ADD ([username])
ALTER FULLTEXT INDEX ON [dbo].[App_User] ADD ([surname])
ALTER FULLTEXT INDEX ON [dbo].[App_User] ENABLE


