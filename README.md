MPI-kth-smallest-element-of-a-List
==================================

Algorithm for obtaining the kth smallest element, assuming p processors each with local memory n/p . This is implemented for Distributed memory systems using Message Passing Interface implementation for .net

Implementation of algorithm on Mpi.net and C# based on Paper : 

A Note on Parallel Selection on Coarse-Grained Multicomputers, E. L. G. Saukas and S. W. Song (1999)
==========================================

Consider the selection problem of determining the kth smallest element of a set of n elements. Under the CGM (coarse-grained multicomputer) model with p processors and O(n/p) local memory, we present a deterministic parallel algorithm for the selection problem that requires O(log p) communication rounds. Besides requiring a low number of communication rounds, the algorithm also attempts to minimize the total amount of data transmitted in each round (only O(p) except in the last round). In addition to showing theoretical complexities, we present very promising experimental results obtained on a parallel machine that show almost linear speedup, indicating the efﬁciency and scalability of the proposed algorithm.

CGM algorithm for obtaining the kth smallest element, assuming p processors each with local memory of size O(n/p).
Input: Set A of n elements distributed among the p processors, each processor i with ni = O(n/p) elements, and an integer k,1≤ k ≤ n. 
Output: An element ai of A such that rank(ai, A) = k. 

* (1) Set N := n 
* (2) Repeat until N ≤ n/(cp) 
* (2.1) Each processor i computes the median mi of its ni elements 
* (2.2) Each processor i sends mi and ni to processor 1 
* (2.3) Processor 1 computes the weighted median M 
* (2.4) Processor 1 broadcasts M to all other processors 
* (2.5) Each processor i computes li,ei, gi, respectively the numbers of its local elements less than, equal to, or greater than M 
* (2.6) Each processor i sends li,ei, gi to processor 1 
* (2.7) Processor 1 computes L =P1≤i≤p li, E =P1≤i≤p ei, G =P1≤i≤p gi, respectively the total numbers of elements less than, equal to, or greater than M 
* (2.8) Processor 1 broadcasts L, E, G to all other processors 
* (2.9) One of the following: if L < k ≤ L + E then return solution M and stop if k ≤ L then each processor i discards all but those elements less than M and set N := L if k > L + E then each processor i discards all but those elements greater than M and set N := G and k := k −(L + E) 
* (3) All the remaining N elements are sent to processor 1 
* (4) Processor 1 solves the remaining problem sequentially

