(defpackage :user (:use :common-lisp))
(in-package :user)
(compile-file "procura.lisp" :print NIL)
(load "procura")
(compile-file "G001.lisp" :print NIL)
(load "G001")
(load "problems")
(require :sb-sprof)

(defun test-successors (problem)
    (let ((initial-state (make-afectacao-state :problem (make-array (length problem) :initial-contents problem) :non-allocated (loop for n from 0 below (length problem) by 1 collect n) :cost 0)))
        (print (successors initial-state))
        (print (successors (first (successors initial-state))))
        (print (successors (second (successors (first (successors initial-state))))))
;;         (print (first (successors initial-state)))
;;         (print (get-shifts-from-state (first (successors initial-state))))
;;         (print (first (successors (fifth (successors (first (successors initial-state)))))))
;;         (print (get-shifts-from-state (first (successors (fifth (successors (first (successors initial-state))))))))
        ))

(defun test (problem strategy)
    (let ((solution))
        (time (setf solution (faz-afectacao problem strategy :stats T)))
        (print solution)
        (format t "~%Solution length: ~D (problem length: ~D)~%" (length solution) (length problem))))

(defun profile (problem strategy)
    (sb-sprof:with-profiling (:mode :alloc :report :flat)
        (faz-afectacao problem strategy)))

(defun auto-test-length (problem strategy expected-length)
    (let ((solution))
        (setf solution (faz-afectacao problem strategy))
        (if (= (length solution) expected-length)
            (progn
;;                 (format t "~%OK: ")
;;                 (prin1 problem)
;;                 (format t " [length: ~D]~%" (length solution))
            )
            (progn
                (format t "~%=======> FAILED: ")
                (prin1 problem)
                (format t " [expected: ~D; actual: ~D] <=======~%" expected-length (length solution))))))

        
;; (time (test-successors p0))

;; In figure 3 of the handout it's "faz-afectação" :S
(print "melhor.abordagem")
(test p0 "melhor.abordagem")
(print "a*.melhor.heuristica")
(test p0 "a*.melhor.heuristica")
(print "a*.melhor.heuristica.alternativa")
(test p0 "a*.melhor.heuristica.alternativa")
(print "sondagem.iterativa")
(test p0 "sondagem.iterativa")
(print "ILDS")
(test p0 "ILDS")
(print "abordagem.alternativa")
(test p0 "abordagem.alternativa")
;; Respective solution (notice the parentheses' balancing!):
;; (((L2 L1 1 25) (L1 L2 34 60)) ((L5 L1 408 447) (L1 L1 448 551)) ((L1 L1 474 565)))
;; ((0 1) (2 3) (4)) -- cost: 1080


;; xor experiments
;; (print (logxor (sxhash '(0 1)) (sxhash '(2 3))))
;; (print (logxor (sxhash '(0 1)) (logxor (logxor (sxhash '(2 3)) (sxhash '(4 5))) (sxhash '(4 5)))))
;; (print (= 786692038858216898 (logxor (logxor 539153531184204986 585654940673594664) 410174370634861648)))
;; (print (= 2056805071380800366 (logxor 585654940673594664 409748454801600428 1226157002546502122)))

;; Switch-a-roo test
(auto-test-length '((L1 L2 0 10) (L2 L3 20 30)) "melhor.abordagem" 1)
(auto-test-length '((L2 L3 20 30) (L1 L2 0 10)) "melhor.abordagem" 1) ;; The crazy-heuristics fail here

;; Restriction 2
(auto-test-length '((L1 L1 1 240) (L1 L1 420 480)) "melhor.abordagem" 1) ;; (maximum shift time of 480 minutes)
(auto-test-length '((L1 L1 1 240) (L1 L1 420 482)) "melhor.abordagem" 2) ;; (over maximum shift time of 480 minutes)
(auto-test-length '((L1 L1 1 240) (L1 L2 420 440)) "melhor.abordagem" 1) ;; (maximum shift time of 480 minutes but needs 40 minutes travel at the end)
(auto-test-length '((L2 L1 1 200) (L1 L1 420 430)) "melhor.abordagem" 1) ;; (maximum shift time of 480 minutes but needs 40 minutes travel at the start)
(auto-test-length '((L1 L1 1 240) (L1 L1 241 480)) "melhor.abordagem" 2) ;; (maximum shift time of 480 minutes but needs meal)
(auto-test-length '((L1 L1 1 240) (L1 L1 400 480)) "melhor.abordagem" 1) ;; (maximum shift time of 480 minutes and has time for meal)

;; Restriction 4
(auto-test-length '((L1 L1 566 633) (L1 L1 684 735) (L1 L1 736 823) (L1 L1 866 933)) "melhor.abordagem" 2)
(auto-test-length '((L1 L1 566 633) (L1 L1 684 735) (L1 L1 736 823) (L1 L1 866 933)) "melhor.abordagem" 2)
(auto-test-length '((L1 L1 994 1085) (L1 L1 1152 1247) (L1 L1 1248 1351) (L1 L2 1394 1420)) "melhor.abordagem" 2)

;; Restriction 5
;; Can't be tested. And it is hardcoded in the code, so it is verified.

;; Restriction 6
;; Already tested with Restriction 7.

;; Restriction 7
(auto-test-length '((L1 L1 1 230) (L1 L1 231 240)) "melhor.abordagem" 1) ;; (maximum of 240 minutes - R7)
(auto-test-length '((L1 L1 1 230) (L1 L1 231 250)) "melhor.abordagem" 2) ;; (violates R7)
(auto-test-length '((L1 L1 1 230) (L1 L1 270 300)) "melhor.abordagem" 1) ;; (has 40 minute interval - R5)
(auto-test-length '((L1 L2 1 190) (L1 L1 269 320)) "melhor.abordagem" 2) ;; (violates R6 as it also needs 40 min travel time)
(auto-test-length '((L1 L2 1 190) (L1 L1 270 320)) "melhor.abordagem" 1) ;; (has 40 minute interval + 40 min travel time)
(auto-test-length '((L2 L3 41 200) (L3 L1 239 300)) "melhor.abordagem" 2) ;; (needs 40 minutes to go from L1 to L2 and 40 minutes for lunch)
(auto-test-length '((L2 L3 41 200) (L3 L1 240 300)) "melhor.abordagem" 1) ;; (has time to go from L1 to L2 and for lunch)

;; R1/8b (using R7 to validate)
(auto-test-length '((L1 L2 1 120) (L2 L1 121 240)) "melhor.abordagem" 1)
(auto-test-length '((L3 L2 1 120) (L2 L1 121 240)) "melhor.abordagem" 2)
(auto-test-length '((L1 L2 1 120) (L2 L3 121 240)) "melhor.abordagem" 2)
(auto-test-length '((L2 L3 41 200) (L3 L1 201 260)) "melhor.abordagem" 2) ;; (needs lunch because of 40 minutes from L1 to L2 at the start)
(auto-test-length '((L1 L2 240 300) (L2 L3 301 460)) "melhor.abordagem" 2) ;; (needs lunch because of 40 minutes from L3 to L1 at the end)

;; For: basic-heuristic
;; (test '((L2 L1 1 25) (L10 L1 4 33) (L1 L2 14 40) (L1 L11 14 55) (L4 L1 16 37)
;; (L2 L1 21 45) (L1 L10 26 55) (L1 L9 28 72) (L1 L2 34 60) (L1 L10 46 75)
;; (L10 L1 364 393)) "melhor.abordagem")

;; Problema-001
;; -Tem 82 tarefas.
;; -Há solução com 20 turnos.
(test p1 "melhor.abordagem")

;; Problema-002
;; -Tem 157 tarefas.
;; -Há solução com 40 turnos.
(test p2 "melhor.abordagem")

;; Problema-003
;; -Tem 231 tarefas.
;; -Há solução com 60 turnos.
(test p3 "melhor.abordagem")

;; Problema-004
;; -Tem 455 tarefas.
;; -Há solução com 120 turnos.
(test p4 "melhor.abordagem")

;; Problema-005
;; -Tem 693 tarefas.
;; -Há solução com 180 turnos.
(test p5 "melhor.abordagem")
