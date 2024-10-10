# RUSConvert
Conversion des exportations Twizzit.
- Factures vers Popsy
- Enregistrements vers CODA SEPA Credit Transfer

## Factures vers Popsy

### Configuration
D�finir les dossiers dans RUSConvert.dll.config
- InvoicesSourceFolder
- InvoicesDestFolder

### R�gles de d�fintion de comptes de produit
Le compte de produit peut �tre d�fini dans les factures Twizzit.
Si le compte n'est pas d�fini, il est possible de d�finir le compte sur base de r�gles bas�es sur la description et le montant.
Fichier de r�gles: \Files\Rules.xlsx
Toutes les colonnes sont obligatoires.
La colonne Account doit �tre au format Texte donc pr�fix�e par '

### Exportation des factures
Dans Twizzit, exporter les factures en format Excel en cochant "inclure d�tails du document" et " inclure liste de destinataires".
Au 09/10/2025, la colonne VCS n'est pas int�gr�e dans la version Excel. Il faut exporter la version Winbooks et copier la colonne VCS en derni�re colonne de l'export Excel.
Exemple de fichier export�: \Files\TwizzitInvoicesExport.xlsx

### Importation dans Popsy
Importer les factures via le menu Outils/Facturation �l�ctronique/Import Ventes.
Les factures sont import�es dans un journal temporaire: Saisies/Ventes/Ventes (facturation �l�ctronique)
Les factures doivent �tre valid�es (Alt-V) une par une pour entrer dans le journal de vente comptable.
Les comptes clients sont cr��s automatiquement par Popsy. Le "Recipient Id" de Twizzit est repris dans le num�ro de TVA, le nom du client est suffix� du "Recipient Id" entre parenth�ses.

## Enregistrements vers CODA SEPA Credit Transfer

### Configuration
D�finir les dossiers dans RUSConvert.dll.config
- PaymentsSourceFolder
- PaymentsDestFolder
- CompanyName
- CompanyIBAN
- CompanyVAT

### Exportation des Enregistrements
Dans Twizzit, exporter les enregistrement en format Excel

