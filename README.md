# BDD - Projet Vote

Contributeur : Emeric SAUTHIER

## Cas d'utilisation

### Cas n°1 - Un candidat obtient plus de 50% des votes
Si un candidat obtient plus de 50% des votes, alors il gagne automatiquement le scrutin, peu importe le tour => majorité absolue

### Cas n°2 - Aucun candidat n'a plus de 50% des votes
Si aucun candidat n'a plus de 50% des votes, alors :
- au 2e tour, c'est celui qui a le plus grand pourcentage qui gagne => majorité relative
- au 1er tour, les deux candidats avec le plus grand pourcentage vont au 2e tour

### Cas n°3 - Egalité
Si deux candidats sont à égalité au 2e tour, aucun vainqueur n'est désigné.

### Cas n°4 - Désignation du vainqueur
La désignation du vainqueur dépend du résultat du tour (voir cas ci-dessus), et de l'état du vote. Il est possible de désigner un vainqueur uniquement pour les votes clotûrés.

### Cas n°5 - Ouverture d'un vote
Pour ouvrir un vote et lancer un tour, il faut :
- Que le vote soit en attente (InComing)
- Que des candidats soient affectés au vote

### Cas n°6 - Clotûre d'un vote
Pour clotûrer un vote, il faut que le vote soit en cours (Opened).

### Cas n°7 - Égalité au 1er tour (2e / 3e candidat)
Le 2e tour ne compte que **2 candidats**. Si plusieurs candidats sont à égalité de voix pour la 2e place qualificative, on départage par l'âge : le **doyen** (date de naissance la plus ancienne) est qualifié.

### Cas n°8 - Vote blanc
Les votes blancs sont comptabilisés et affichés, mais ne peuvent pas gagner. Ils sont **exclus du calcul de la majorité** : le seuil de 50% et les pourcentages des candidats sont calculés sur les suffrages exprimés (total hors blancs).

## Modèle
Un candidat est représenté par la classe `Candidate` (nom + date de naissance). La date de naissance sert uniquement au départage des égalités (voir Cas n°7).