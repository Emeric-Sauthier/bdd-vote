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