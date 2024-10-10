# RUSConvert
Conversion des exportations Twizzit.
- Factures vers Popsy
- Enregistrements vers CODA SEPA Credit Transfer

## Factures vers Popsy

### Configuration
Définir les dossiers dans RUSConvert.dll.config
- InvoicesSourceFolder
- InvoicesDestFolder

### Règles de défintion de comptes de produit
Le compte de produit peut être défini dans les factures Twizzit.
Si le compte n'est pas défini, il est possible de définir le compte sur base de règles basées sur la description et le montant.
Fichier de règles: \Files\Rules.xlsx
Toutes les colonnes sont obligatoires.
La colonne Account doit être au format Texte donc préfixée par '

### Exportation des factures
Dans Twizzit, exporter les factures en format Excel en cochant "inclure détails du document" et " inclure liste de destinataires".
Au 09/10/2025, la colonne VCS n'est pas intégrée dans la version Excel. Il faut exporter la version Winbooks et copier la colonne VCS en dernière colonne de l'export Excel.
Exemple de fichier exporté: \Files\TwizzitInvoicesExport.xlsx

### Importation dans Popsy
Importer les factures via le menu Outils/Facturation éléctronique/Import Ventes.
Les factures sont importées dans un journal temporaire: Saisies/Ventes/Ventes (facturation éléctronique)
Les factures doivent être validées (Alt-V) une par une pour entrer dans le journal de vente comptable.
Les comptes clients sont créés automatiquement par Popsy. Le "Recipient Id" de Twizzit est repris dans le numéro de TVA, le nom du client est suffixé du "Recipient Id" entre parenthèses.

## Enregistrements vers CODA SEPA Credit Transfer

### Configuration
Définir les dossiers dans RUSConvert.dll.config
- PaymentsSourceFolder
- PaymentsDestFolder
- CompanyName
- CompanyIBAN
- CompanyVAT

### Exportation des Enregistrements
Dans Twizzit, exporter les enregistrement en format Excel

